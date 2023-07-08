from typing import List
from fastapi import FastAPI
from fastapi.responses import StreamingResponse
from transformers import AutoTokenizer, AutoModel
from pydantic import BaseModel
import uvicorn, json, torch

class Message(BaseModel):
    role: str
    content: str

class RequestSettings(BaseModel):
    temperature: float
    maxResponseTokens: int
    topP: float
    frequencyPenalty: float
    presencePenalty: float

class CopilotChatRequest(BaseModel):
    message: str
    history: List[Message]
    settings: RequestSettings

class CopilotTextRequest(BaseModel):
    message: str
    settings: RequestSettings

class CopilotMessageResponse(BaseModel):
    content: str
    isError: bool

def convert_to_string_array(message_list: List[Message]) -> List[tuple[str, str]]:
    string_array = []
    if message_list:
        first_item = message_list[0]
        if first_item.role == 'System':
            string_array.append((first_item.content, 'OK'))
            message_list.pop(0)
    for i in range(0, len(message_list) - 1, 2):
        user_message = message_list[i]
        assistant_message = message_list[i+1]
        user_content = user_message.content if user_message else ""
        assistant_content = assistant_message.content if assistant_message else ""

        string_array.append((user_content, assistant_content))
    return string_array

def torch_gc():
    if torch.cuda.is_available():
        with torch.cuda.device(CUDA_DEVICE):
            torch.cuda.empty_cache()
            torch.cuda.ipc_collect()

DEVICE = "cuda"
DEVICE_ID = "0"
CUDA_DEVICE = f"{DEVICE}:{DEVICE_ID}" if DEVICE_ID else DEVICE

app = FastAPI()

@app.post("/chat", response_model=CopilotMessageResponse)
async def chat_response(request: CopilotChatRequest):
    global model, tokenizer
    model_history = convert_to_string_array(request.history)
    model_max_length = request.settings.maxResponseTokens + base_token_count
    model_top_p = request.settings.topP if request.settings.topP else 0.7
    model_temperature = request.settings.temperature if request.settings.temperature else 0.95
    try:
        response, history = model.chat(tokenizer,
                                    request.message,
                                    history=model_history,
                                    max_length=model_max_length,
                                    top_p=model_top_p,
                                    temperature=model_temperature)
        answer = CopilotMessageResponse(content=response, isError=False)
    except Exception as ex:
        answer = CopilotMessageResponse(content=str(ex), isError=True)
    torch_gc()
    return answer

@app.post("/chat-stream")
async def chat_stream_response(request: CopilotChatRequest):
    global model, tokenizer
    model_history = convert_to_string_array(request.history)
    model_max_length = request.settings.maxResponseTokens + base_token_count
    model_top_p = request.settings.topP if request.settings.topP else 0.7
    model_temperature = request.settings.temperature if request.settings.temperature else 0.95

    async def generate_stream():
        try:
            for response, history in model.stream_chat(tokenizer, request.message, history=model_history, max_length=model_max_length, top_p=model_top_p, temperature=model_temperature):
                global stop_chat_stream
                if stop_chat_stream:
                    stop_chat_stream = False
                    break
                else:
                    temp = {"content":response,"isError":False}
                    yield f"{json.dumps(temp)}\n"
            yield f"[DONE]"
        except Exception as ex:
            yield f"error: {ex}\n\n"
            
    torch_gc()
    return StreamingResponse(generate_stream(), media_type="text/event-stream")

@app.post("/chat-stop")
async def chat_stream_stop():
    global stop_chat_stream
    stop_chat_stream = True

@app.post("/text", response_model=CopilotMessageResponse)
async def text_response(request: CopilotTextRequest):
    global model, tokenizer
    model_max_length = request.settings.maxResponseTokens + base_token_count
    model_top_p = request.settings.topP if request.settings.topP else 0.7
    model_temperature = request.settings.temperature if request.settings.temperature else 0.95
    try:
        response, history = model.chat(tokenizer,
                                    request.message,
                                    history=[],
                                    max_length=model_max_length,
                                    top_p=model_top_p,
                                    temperature=model_temperature)
        answer = CopilotMessageResponse(content=response, isError=False)
    except Exception as ex:
        answer = CopilotMessageResponse(content=str(ex), isError=True)
    torch_gc()
    return answer

@app.post("/text-stream")
async def text_stream_response(request: CopilotTextRequest):
    global model, tokenizer
    model_max_length = request.settings.maxResponseTokens + base_token_count
    model_top_p = request.settings.topP if request.settings.topP else 0.7
    model_temperature = request.settings.temperature if request.settings.temperature else 0.95
    async def generate_stream():
        try:
            for response, history in model.stream_chat(tokenizer, request.message, history=[], max_length=model_max_length, top_p=model_top_p, temperature=model_temperature):
                global stop_text_stream
                if stop_text_stream:
                    stop_text_stream = False
                    break
                else:
                    temp = {"content":response,"isError":False}
                    yield f"{json.dumps(temp)}\n"
            yield f"[DONE]"
        except Exception as ex:
            yield f"error: {ex}\n\n"
    torch_gc()
    return StreamingResponse(generate_stream(), media_type="text/event-stream")

@app.post("/text-stop")
async def text_stream_stop():
    global stop_text_stream
    stop_text_stream = True

if __name__ == '__main__':
    stop_chat_stream = False
    stop_text_stream = False
    tokenizer = AutoTokenizer.from_pretrained("model", trust_remote_code=True, revision="")
    model = AutoModel.from_pretrained("model", trust_remote_code=True, revision="").half().cuda()
    model.eval()
    base_token_count = 2048
    with open('config.json', 'r') as f:
        config = json.load(f)
        if 'base_token_count' in config:
            base_token_count = config['base_token_count']
    uvicorn.run(app, host='0.0.0.0', port=8000, workers=1)
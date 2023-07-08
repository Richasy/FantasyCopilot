from typing import List
from InstructorEmbedding import INSTRUCTOR
from fastapi import FastAPI, Request
from pydantic import BaseModel
import uvicorn

class EmbeddingData(BaseModel):
    embedding: List[float]

app = FastAPI()

@app.post("/embedding", response_model=EmbeddingData)
async def text_response(request: Request):
    global model
    body = await request.body()
    detail = body.decode()
    embeddings =model.encode(detail)
    data = EmbeddingData(embedding=embeddings.tolist())
    return data

if __name__ == '__main__':
    stop_embedding = False
    model = INSTRUCTOR("model")
    uvicorn.run(app, host='0.0.0.0', port=4212, workers=1)
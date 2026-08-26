from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import base64
import cv2
import numpy as np

app = FastAPI(title="AI Face Verify Service")

# Enable CORS for local development (React dev server)
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost",
        "http://127.0.0.1",
        "http://localhost:3000",
        "http://localhost:5173",
        "http://localhost:54543",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


class VerifyRequest(BaseModel):
    image_b64: str


class VerifyResponse(BaseModel):
    ok: bool
    face: bool
    message: str


def decode_image(image_b64: str) -> np.ndarray:
    try:
        header_sep = image_b64.find(",")
        if header_sep != -1:
            image_b64 = image_b64[header_sep + 1 :]
        img_bytes = base64.b64decode(image_b64)
        data = np.frombuffer(img_bytes, dtype=np.uint8)
        img = cv2.imdecode(data, cv2.IMREAD_COLOR)
        return img
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"invalid_image: {e}")


def detect_face(img: np.ndarray) -> bool:
    if img is None:
        return False

    # Downscale very large frames to keep detection stable
    height, width = img.shape[:2]
    max_dim = max(height, width)
    if max_dim > 960:
        scale = 960.0 / max_dim
        img = cv2.resize(img, (int(width * scale), int(height * scale)))

    # Convert to grayscale and normalize contrast for low-light scenes
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    gray = cv2.equalizeHist(gray)

    # Try multiple cascades with slightly relaxed parameters
    cascade_paths = [
        "haarcascade_frontalface_default.xml",
        "haarcascade_frontalface_alt2.xml",
        "haarcascade_profileface.xml",
    ]

    for cascade_name in cascade_paths:
        cascade = cv2.CascadeClassifier(cv2.data.haarcascades + cascade_name)
        if cascade.empty():
            continue
        faces = cascade.detectMultiScale(
            gray,
            scaleFactor=1.05,
            minNeighbors=3,
            minSize=(60, 60),
        )
        if len(faces) > 0:
            return True

    return False


@app.post("/verify", response_model=VerifyResponse)
def verify(req: VerifyRequest):
    img = decode_image(req.image_b64)
    has_face = detect_face(img)
    ok = bool(has_face)
    message = "face_detected" if has_face else "no_face"
    return VerifyResponse(ok=ok, face=has_face, message=message)


@app.get("/health")
def health():
    return {"status": "ok"}



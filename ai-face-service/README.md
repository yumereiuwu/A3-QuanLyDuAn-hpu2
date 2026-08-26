# AI Face Verify Service

## Run locally

```bash
python -m venv .venv
. .venv/Scripts/activate  # Windows PowerShell: .venv\Scripts\Activate.ps1
pip install -r requirements.txt
uvicorn main:app --host 0.0.0.0 --port 9090
```

## API
- POST `/verify` { image_b64: dataUrl, require_no_mask: boolean }
  - Response: { ok, face, masked, message }
- GET `/health`



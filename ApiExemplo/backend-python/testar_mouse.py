"""
Script de diagnóstico via HTTP - testa o endpoint /api/detectar sem precisar da câmera.
Captura um screenshot da tela e envia para o backend para ver o que o mouse.pt detecta.
"""
import base64
import json
import urllib.request
import urllib.error
import numpy as np
import cv2

print("=" * 60)
print("DIAGNÓSTICO VIA HTTP - mouse.pt")
print("=" * 60)

# Testa se o backend está respondendo
print("\n[1] Verificando status do backend...")
try:
    resp = urllib.request.urlopen("http://localhost:8000/api/status", timeout=3)
    dados = json.loads(resp.read())
    print(f"    Backend OK: {dados}")
except Exception as e:
    print(f"    ERRO: backend não responde - {e}")
    exit(1)

# Tira um screenshot da tela atual (onde provavelmente tem um mouse visível)
print("\n[2] Capturando screenshot da tela...")
try:
    import subprocess
    # Usa PowerShell para capturar screenshot
    script_ps = """
Add-Type -AssemblyName System.Windows.Forms
$screen = [System.Windows.Forms.Screen]::PrimaryScreen
$bmp = New-Object System.Drawing.Bitmap($screen.Bounds.Width, $screen.Bounds.Height)
$gfx = [System.Drawing.Graphics]::FromImage($bmp)
$gfx.CopyFromScreen($screen.Bounds.Location, [System.Drawing.Point]::Empty, $screen.Bounds.Size)
$bmp.Save('C:/Users/Aluno/Downloads/AAAA/ApiExemplo/backend-python/teste_screenshot.png')
$gfx.Dispose()
$bmp.Dispose()
Write-Output "Screenshot salvo"
"""
    resultado = subprocess.run(["powershell", "-Command", script_ps], capture_output=True, text=True, timeout=10)
    print(f"    {resultado.stdout.strip()}")
    
    # Carrega o screenshot
    imagem = cv2.imread("teste_screenshot.png")
    if imagem is None:
        raise Exception("Não conseguiu carregar screenshot")
    print(f"    Screenshot: {imagem.shape[1]}x{imagem.shape[0]} pixels")
except Exception as e:
    print(f"    Erro no screenshot: {e}")
    # Cria imagem sintética cinza como fallback
    print("    Usando imagem de fallback...")
    imagem = np.zeros((480, 640, 3), dtype=np.uint8)
    imagem[:] = (128, 128, 128)

# Codifica a imagem em base64
print("\n[3] Codificando imagem...")
_, buffer = cv2.imencode('.jpg', imagem, [cv2.IMWRITE_JPEG_QUALITY, 85])
imagem_b64 = base64.b64encode(buffer).decode('utf-8')
print(f"    Tamanho do base64: {len(imagem_b64)} chars")

# Envia para o backend (com log detalhado no servidor)
print("\n[4] Enviando para /api/detectar...")
payload = json.dumps({"imagem": imagem_b64}).encode('utf-8')
req = urllib.request.Request(
    "http://localhost:8000/api/detectar",
    data=payload,
    headers={"Content-Type": "application/json"},
    method="POST"
)
try:
    resp = urllib.request.urlopen(req, timeout=15)
    resultado = json.loads(resp.read())
    deteccoes = resultado.get("deteccoes", [])
    print(f"\n[5] Detecções retornadas pelo backend: {len(deteccoes)}")
    if deteccoes:
        for d in deteccoes:
            print(f"    - class_name: {d['class_name']} | class_id: {d['class_id']} | confiança: {d['confidence']:.1%}")
    else:
        print("    Nenhuma detecção. O modelo não encontrou nada na imagem.")
        print("\n    CONCLUSÃO: Provavelmente é o modelo - precisa de melhor treinamento ou")
        print("    a imagem de teste não tem o objeto que ele foi treinado para detectar.")
except Exception as e:
    print(f"    ERRO na requisição: {e}")

# Testa também isolado o mouse.pt para ver threshold
print("\n" + "=" * 60)
print("TESTE DIRETO: mouse.pt com confiança 1%")
print("=" * 60)
from ultralytics import YOLO
modelo = YOLO("mouse.pt")
resultados = modelo(imagem, conf=0.01, verbose=False)
total = 0
for r in resultados:
    for caixa in r.boxes:
        classe_id = int(caixa.cls[0].item())
        confianca = float(caixa.conf[0].item())
        print(f"    Detectado: {modelo.names[classe_id]} com {confianca:.1%}")
        total += 1
if total == 0:
    print("    Nenhuma detecção mesmo com conf=1%")
    print("    -> PROBLEMA É O MODELO TREINADO (não reconhece o objeto na imagem)")
else:
    print(f"    Total: {total} detecções")
    print("    -> O MODELO FUNCIONA, mas o threshold do backend pode estar alto demais")
    
print("\nDiagnóstico finalizado!")

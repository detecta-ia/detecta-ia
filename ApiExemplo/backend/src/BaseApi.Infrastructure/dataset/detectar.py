import sys
import argparse
from ultralytics import YOLO

def executar_deteccao():
    # 1. Configura o script para aceitar o caminho da imagem vindo do C#
    parser = argparse.ArgumentParser()
    parser.add_argument('--source', type=str, required=True, help='Caminho da imagem para detectar')
    args = parser.parse_args()

    try:
        # 2. Carrega o modelo que VOCÊ treinou (ajuste o caminho se a pasta for train-2, train-3, etc.)
        modelo = YOLO("runs/detect/train/weights/best.pt")

        # 3. Roda a IA na imagem enviada pelo C#
        # conf=0.50 significa que ele só avisa se tiver mais de 50% de certeza
        resultados = modelo.predict(source=args.source, conf=0.50, verbose=False)

        # 4. Lê o que a IA encontrou
        for resultado in resultados:
            caixas = resultado.boxes
            if len(caixas) == 0:
                print("Nenhum objeto detectado.")
                return

            for caixa in caixas:
                # Pega o nome da classe (ex: 'mouse') e a confiança (ex: 0.92)
                cls_id = int(caixa.cls[0])
                nome_classe = modelo.names[cls_id]
                confianca = float(caixa.conf[0])
                
                # O C# vai ler este 'print' para saber o resultado!
                print(f"Detectado: {nome_classe} ({confianca * 100:.1f}%)")

    except Exception as e:
        print(f"Erro na deteccao: {str(e)}")

if __name__ == "__main__":
    executar_deteccao()
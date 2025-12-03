# PokerStars Accessibility

Software de acessibilidade para PokerStars que utiliza OCR para extrair e disponibilizar informações do jogo de forma acessível.

## Funcionalidades

- **OCR do PokerStars**: Captura e analisa a tela do PokerStars para extrair informações do jogo
- **Listagem de ações**: Exibe textualmente todas as ações do jogo em tempo real
- **Exibição de fichas**: Mostra a quantidade de fichas do jogador e pot atual
- **Blinds**: Exibe small blind e big blind da mesa
- **Cartas do jogador**: Mostra as cartas do jogador (hole cards)
- **Ações dos jogadores**: Lista todas as ações dos outros jogadores
- **Text-to-Speech**: Leitura de texto para maior acessibilidade

## Estrutura do Projeto

```
PokerStars.Accessibility/
├── src/
│   ├── PokerStars.Accessibility.Models/     # Modelos de dados
│   ├── PokerStars.Accessibility.Core/       # Lógica de OCR e processamento
│   ├── PokerStars.Accessibility.Services/   # Serviços (OCR, TTS, extração)
│   └── PokerStars.Accessibility.Maui/       # Aplicação MAUI Blazor Hybrid
├── tests/
│   └── PokerStars.Accessibility.Tests/      # Testes unitários
├── Directory.Build.props
└── PokerStars.Accessibility.sln
```

## Projetos

### PokerStars.Accessibility.Models
Contém os modelos de dados:
- `Card`: Representa uma carta de baralho (rank e naipe)
- `Player`: Representa um jogador na mesa
- `PlayerAction`: Representa uma ação (fold, call, raise, etc.)
- `Table`: Representa a mesa de poker
- `GameState`: Estado completo do jogo

### PokerStars.Accessibility.Core
Contém a lógica de processamento:
- `PokerTextParser`: Analisa texto OCR e extrai dados do jogo
- `OcrResult`: Resultado de operações OCR
- `ScreenCapture`: Dados de captura de tela

### PokerStars.Accessibility.Services
Contém os serviços:
- `IScreenCaptureService` / `WindowsScreenCaptureService`: Captura de tela no Windows
- `IOcrService` / `OcrService`: Serviço de OCR
- `IGameExtractionService` / `GameExtractionService`: Extração de dados do jogo
- `ITextToSpeechService` / `MauiTextToSpeechService`: Text-to-Speech

### PokerStars.Accessibility.Maui
Aplicação MAUI Blazor Hybrid para Windows com:
- Página principal com estado do jogo
- Página de histórico de ações
- Página de configurações (TTS, acessibilidade)

## Requisitos

- .NET 9.0
- Visual Studio 2022 ou Visual Studio Code
- Windows 10/11 (para build completo do MAUI)
- MAUI workload instalado

## Como Compilar

### Instalar workload MAUI
```bash
dotnet workload install maui-windows
```

### Restaurar pacotes
```bash
dotnet restore
```

### Compilar bibliotecas core (funciona em qualquer SO)
```bash
dotnet build src/PokerStars.Accessibility.Models
dotnet build src/PokerStars.Accessibility.Core
```

### Compilar projeto MAUI (requer Windows)
```bash
dotnet build src/PokerStars.Accessibility.Maui
```

### Executar testes
```bash
dotnet test
```

## Como Usar

1. Abra o PokerStars e entre em uma mesa
2. Execute a aplicação PokerStars Accessibility
3. Clique em "Monitor" para iniciar a captura automática
4. Use "Speak State" para ouvir o estado atual do jogo
5. Configure as preferências de TTS em "Settings"

## Tecnologias

- .NET 9
- Blazor Hybrid
- MAUI
- xUnit (testes)

## Acessibilidade

A aplicação foi desenvolvida com foco em acessibilidade:
- Todos os elementos têm `aria-label` descritivos
- Suporte a navegação por teclado
- Text-to-Speech integrado
- Alto contraste suportado
- Animações reduzidas opcionalmente

## Licença

Este projeto é apenas para fins educacionais e de acessibilidade.

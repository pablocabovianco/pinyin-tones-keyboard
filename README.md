# Pinyin Tones

[English](#english) · [Español](#español)

---

## English

A small Windows tray utility for typing pinyin with tone marks. Type a syllable followed by its tone number and it is replaced in place, in any application:

| You type | You get |
|---|---|
| `hao3` | hǎo |
| `ni3hao3` | nǐhǎo |
| `lv4` | lǜ |
| `nar3` | nǎr |
| `ma5` | ma (neutral tone) |

- Use `v` for `ü`.
- Tones 1–4 add the mark; 5 is the neutral tone (the number is just removed).
- Erhua syllables work too: the mark goes on the base syllable (`zher4` → zhèr, `dianr3` → diǎnr).
- Capitals are kept (`HAO3` → HǍO). Numpad digits work too.
- The tone is whatever key types the digit on your keyboard layout (on AZERTY, Shift+1…5).
- Anything that isn't a valid pinyin syllable is left alone, so `mp3` or `win10` stay as typed.

### Download

Get `PinyinTones.exe` from the [latest release](../../releases/latest). It's a single file with no installer, and it doesn't need .NET installed.

The executable isn't code-signed, so Windows SmartScreen may show *"Windows protected your PC"* the first time. Click *More info* → *Run anyway*. Releases are built from this repository by [GitHub Actions](.github/workflows/build.yml).

### Usage

Run `PinyinTones.exe`. A red **ǎ** appears in the system tray. Double-click it (or use the menu) to pause; it turns grey. Right-click → *Exit* to quit. The menu follows the Windows display language (Spanish or English).

### Privacy

The app watches keystrokes only to detect the syllable being typed. It keeps at most the last 7 letters in memory, and the buffer is cleared on any non-letter key, mouse click or window change. Nothing is stored or sent anywhere.

### Requirements

Windows 10 or 11.

### Building

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet build
dotnet test
dotnet publish UI -c Release -r win-x64 --self-contained true
```

The last command produces a single `PinyinTones.exe` in `UI/bin/Release/net8.0-windows/win-x64/publish/`.

### Known limitations

- It doesn't convert inside applications running as administrator unless it is run as administrator too.
- Some antivirus tools may flag it, because it uses a global keyboard hook.

### License

[MIT](LICENSE)

---

## Español

Una pequeña utilidad para Windows, en la bandeja del sistema, que sirve para escribir pinyin con marcas de tono. Escribís la sílaba seguida del número de tono y se reemplaza ahí mismo, en cualquier programa:

| Escribís | Obtenés |
|---|---|
| `hao3` | hǎo |
| `ni3hao3` | nǐhǎo |
| `lv4` | lǜ |
| `nar3` | nǎr |
| `ma5` | ma (tono neutro) |

- La `v` se usa para la `ü`.
- Los tonos 1 a 4 agregan la marca; el 5 es el tono neutro (solo se quita el número).
- También funcionan las sílabas con erhua: la marca va en la sílaba base (`zher4` → zhèr, `dianr3` → diǎnr).
- Respeta las mayúsculas (`HAO3` → HǍO). También funcionan los números del teclado numérico.
- El tono es la tecla que escribe el número en tu distribución de teclado (en AZERTY, Shift+1…5).
- Lo que no es una sílaba de pinyin válida queda tal cual, así que `mp3` o `win10` no cambian.

### Descarga

Descargá `PinyinTones.exe` desde la [última release](../../releases/latest). Es un único archivo, sin instalador, y no necesita tener .NET instalado.

El ejecutable no está firmado, así que la primera vez Windows SmartScreen puede mostrar *"Windows protegió su PC"*. Hacé clic en *Más información* → *Ejecutar de todas formas*. Las releases se compilan desde este repositorio con [GitHub Actions](.github/workflows/build.yml).

### Uso

Ejecutá `PinyinTones.exe`. Aparece una **ǎ** roja en la bandeja del sistema. Con doble clic (o desde el menú) se pausa y se pone gris. Clic derecho → *Salir* para cerrarla. El menú sigue el idioma de Windows (español o inglés).

### Privacidad

La aplicación mira el teclado solo para detectar la sílaba que se está escribiendo. Guarda en memoria como mucho las últimas 7 letras, y las descarta con cualquier tecla que no sea letra, con un clic o al cambiar de ventana. No guarda ni envía nada.

### Requisitos

Windows 10 u 11.

### Compilación

Necesita el [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet build
dotnet test
dotnet publish UI -c Release -r win-x64 --self-contained true
```

El último comando genera un único `PinyinTones.exe` en `UI/bin/Release/net8.0-windows/win-x64/publish/`.

### Limitaciones conocidas

- No convierte dentro de programas que se ejecutan como administrador, salvo que también se ejecute como administrador.
- Algunos antivirus pueden marcarla, porque usa un hook global de teclado.

### Licencia

[MIT](LICENSE)

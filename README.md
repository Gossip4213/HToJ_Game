# HToJ Prologue Localization Patch

The latest repository branch was checked directly.

Actual source files:
- `Assets/Story/PrologueEN.ink`
- `Assets/Story/PrologueZH.ink`

The Prologue scene currently references independent English and Chinese compiled Ink
assets. This patch adds the missing localized Resources assets:

- `Assets/Resources/Story/PrologueZH.ink`
- `Assets/Resources/Story/PrologueJP.ink`
- `Assets/Resources/Story/PrologueKR.ink`

It also replaces `RuntimeInkLocalizationBridge.cs` with a version that supports both:
- `Chapter0_EN` -> `Chapter0_ZH`, `Chapter0_JP`, `Chapter0_KR`
- `PrologueEN` -> `PrologueZH`, `PrologueJP`, `PrologueKR`

## Install

Copy the included `Assets` directory into the Unity project, replacing the older
`RuntimeInkLocalizationBridge.cs` from the previous localization patch.

Open Unity and wait for Ink compilation.

## Verification

Set each profile language and start a new game:

- ZH_CN: Chinese prologue
- JP: Japanese prologue
- KR: Korean prologue

Each version must then load `Chapter0_Test`.

Speaker tags remain `Ambrose` and `The Judge` intentionally, so the current automatic
portrait mapping continues to work without further ScenarioManager changes.

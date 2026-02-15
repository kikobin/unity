# PNG Theme Setup

## What is already done
- `PngThemeRuntimeApplier` auto-starts in every scene.
- It applies:
  - Backgrounds per scene: `MainMenu`, `Game`, `Results`
  - Button sprite for all `Button` components
  - Gameplay sprites: `Player`, `Enemy`, `Fireball` (projectile)

## One-time setup
1. In Unity menu, click `Tools -> Art -> Create PNG Theme Profile`.
2. Open created asset: `Assets/Resources/PngThemeProfile.asset`.
3. Drag your PNG sprites into fields:
   - `Main Menu Background`
   - `Game Background`
   - `Results Background`
   - `Button Sprite` (общий fallback)
   - `Start Button Sprite`
   - `Quit Button Sprite`
   - `Restart Button Sprite`
   - `Main Menu Button Sprite`
   - `Player Sprite`
   - `Enemy Sprite`
   - `Fireball Sprite`
   - `Button Scale Multiplier` (общий для всех кнопок)
   - `Start/Quit/Restart/Main Menu Button Scale Multiplier` (тонкая настройка)
   - `Hide Button Labels` (скрыть текст поверх кнопок-картинок)
   - `Minimum Button Gap` (минимальный зазор между кнопками в одном ряду)
   - `Use Default Styled Buttons` (рекомендуется: дефолтные красивые кнопки без PNG)
4. If models are too big/small:
   - Keep `Preserve Original World Size` enabled.
   - Tune `Player Scale Multiplier`, `Enemy Scale Multiplier`, `Fireball Scale Multiplier` (e.g. `0.3` to reduce).

## Recommended PNG folders
- `Assets/Art/Backgrounds/MainMenu/`
- `Assets/Art/Backgrounds/Game/`
- `Assets/Art/Backgrounds/Results/`
- `Assets/Art/UI/Buttons/`
- `Assets/Art/Characters/Player/`
- `Assets/Art/Characters/Enemies/`
- `Assets/Art/Projectiles/`

## Notes
- PNG files under `Assets/Art/` are imported as sprites automatically by your import postprocessor.
- If you change the profile while Play Mode is running, sprites refresh automatically.
- If a scene has no background image object, one is created automatically inside the first `Canvas`.
- To hide default placeholder squares/circles in gameplay when sprite is missing, enable `Hide Unassigned Gameplay Placeholders`.
- To hide primitive placeholder sprites (`Square`, `Circle`) automatically, enable `Hide Primitive Placeholder Sprites`.
- To hide default UI button graphic when button sprite is missing, enable `Hide Default Button Graphics`.
- Enemy sprite replacement is forced every frame, so placeholder squares should not be visible before enemy appears.
- If you see outlines/collider shapes only in Scene view, disable `Gizmos` in the Scene window.

# Art Drop Zones

Просто кладите PNG в эти папки:

- `Assets/Art/Backgrounds` - фон для сцены `Game`
- `Assets/Art/Characters/Player` - спрайт игрока (маг)
- `Assets/Art/Characters/Enemies` - спрайты врагов
- `Assets/Art/Projectiles` - файрболлы/снаряды
- `Assets/Art/UI/Buttons/MainMenu` - кнопки главного меню
- `Assets/Art/UI/Buttons/Results` - кнопки экрана результатов
- `Assets/Art/UI/Backgrounds/MainMenu` - фон главного меню
- `Assets/Art/UI/Backgrounds/Results` - фон экрана результатов
- `Assets/Art/Spritesheets` - только спрайт-листы (импортируются как `Multiple`)

## Что настроится автоматически

Для всех PNG внутри `Assets/Art/...`:

- `Texture Type = Sprite (2D and UI)`
- `Sprite Mode = Single`
- `Sprite Mode = Multiple` только в `Assets/Art/Spritesheets`
- `Mesh Type = Full Rect`

## Рекомендуемые имена файлов

- `background_game.png`
- `player_mage.png`
- `enemy_basic.png`
- `projectile_fireball.png`
- `btn_mainmenu_play.png`
- `btn_results_retry.png`
- `ui_background_mainmenu.png`
- `ui_background_results.png`

## Разные фоны для MainMenu и Results

Кладите файлы сюда:

- `Assets/Art/UI/Backgrounds/MainMenu/ui_background_mainmenu.png`
- `Assets/Art/UI/Backgrounds/Results/ui_background_results.png`

В Unity:

1. Откройте сцену `MainMenu`.
2. В `Canvas` создайте `UI -> Image`, назовите `BG_MainMenu`.
3. Для `BG_MainMenu` поставьте Anchors = Stretch (по всему экрану), `Left/Right/Top/Bottom = 0`.
4. В `Image -> Source Image` назначьте `ui_background_mainmenu.png`.
5. Перетащите `BG_MainMenu` в самый верх списка детей `Canvas` (первым), чтобы он был под кнопками/текстом.
6. Откройте сцену `Results`.
7. В `Canvas` создайте `UI -> Image`, назовите `BG_Results`.
8. Так же растяните на весь экран и назначьте `ui_background_results.png`.
9. Перетащите `BG_Results` в самый верх списка детей `Canvas`.

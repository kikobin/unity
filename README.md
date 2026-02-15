# Lite Project Skeleton (CP-01)

Этот шаг создаёт базовый core-каркас Lite-проекта: состояние игры, переходы между сценами и хранение результатов последнего забега.

## Pipeline CP-01...CP-12

- CP-01: Core state + scene transitions + result bridge.
- CP-02...CP-11: поэтапная реализация геймплея, UI, врагов, аудио, полировки и связки систем.
- CP-12: финальная проверка, стабилизация и подготовка к сборке.

## Edit Rules

- Разрешено редактировать только `.cs` и `README.md`.
- Не редактировать `.meta`, `.unity`, `.prefab`, `ProjectSettings/`, `Packages/`.
- Настройка сцен/объектов/Inspector выполняется только через Unity Editor Steps.

## Implemented Core Scripts (CP-01)

- `GameState`: enum состояний `MainMenu`, `Playing`, `Win`, `Lose`, `Results`.
- `GameRoot`: хранит `CurrentState`, `Score`, ссылку на `SceneLoader`; содержит `SetState`, `AddScore`, `EndGame(bool win)`.
- `RunResultStore`: статический bridge последнего результата (`win/lose`, `score`, `wave`, `time`).
- `SceneLoader`: безопасная загрузка сцены по имени + рестарт игровой сцены.

## Unity Editor Steps (ультра-пошагово)

1. Открой Unity проект.
2. В окне `Project` создай папки:
   `Assets/Scenes`, `Assets/Scripts/Core`, `Assets/Scripts/Player`, `Assets/Scripts/Enemies`, `Assets/Scripts/Combat`, `Assets/Scripts/UI`, `Assets/Scripts/Audio`, `Assets/Prefabs`, `Assets/Animations`, `Assets/Audio`.
3. Создай 3 сцены в `Assets/Scenes`: `MainMenu`, `Game`, `Results`.
4. Открой сцену `Game`.
5. В `Hierarchy` нажми `Right Click -> Create Empty`.
6. Переименуй объект в `GameRoot`.
7. Выдели `GameRoot` и нажми `Add Component`.
8. Добавь компонент `GameRoot` (скрипт `GameRoot.cs`).
9. На этом же объекте нажми `Add Component` и добавь `SceneLoader` (или создай отдельный объект `SceneLoader` и добавь компонент туда).
10. Если `SceneLoader` на отдельном объекте: перетащи этот объект в поле `Scene Loader` у `GameRoot` в Inspector.
11. Открой `File -> Build Settings`.
12. Добавь сцены по очереди кнопкой `Add Open Scenes`: `MainMenu`, `Game`, `Results`.
13. Убедись, что `MainMenu` стоит первой в списке (`index 0`).
14. Вернись к `GameRoot` и проверь в Inspector обязательные ссылки (`Scene Loader`) назначены.
15. Нажми `Play` в сцене `Game` и проверь, что нет `NullReferenceException`.
16. Для быстрой проверки логики позже вызови `EndGame(true)` или `EndGame(false)` (через временную кнопку/контекст) и проверь смену состояния и переход к `Results`.

## Acceptance Checks

- При `Play` в сцене `Game` нет `NullReferenceException`.
- `EndGame(true/false)` меняет state и сохраняет результат в `RunResultStore`.
- В `Build Settings` есть 3 сцены: `MainMenu`, `Game`, `Results`.

## Assumption

`RunResultStore` используется как простой static-мост между `Game` и `Results`, без сохранения на диск.

---

# Ranged Attack (CP-03)

Добавлена простая ranged-атака игрока с projectile:

- `PlayerAttack`: стрельба по `Mouse0` (основная) и `Space` (резерв), cooldown, спавн projectile prefab.
- Aim logic:
  `Camera.main + валидная позиция мыши` -> выстрел в курсор;
  иначе fallback в `LastMoveDirection` игрока.
- `Projectile2D`: скорость, lifetime, урон, `OnTriggerEnter2D` по врагам с тегом `Enemy`.
- Projectile уничтожается по таймеру и при попадании.

## Скрипты (CP-03)

- `Assets/Scripts/PlayerAttack.cs`
- `Assets/Scripts/Projectile2D.cs`
- `Assets/Scripts/PlayerController.cs` (добавлен `SafeLastMoveDirection` для безопасного fallback)

## Unity Editor Steps (ультра-пошагово)

1. Открой сцену `Game`.
2. В `Hierarchy` нажми `Right Click -> 2D Object -> Sprites -> Circle` (или любой маленький sprite).
3. Переименуй объект в `Projectile`.
4. Выдели `Projectile`, в `Inspector` нажми `Add Component`.
5. Добавь `Rigidbody2D`.
6. В `Rigidbody2D` поставь:
   `Body Type = Kinematic`
   `Gravity Scale = 0`
7. Добавь `CircleCollider2D`.
8. В `CircleCollider2D` включи `Is Trigger`.
9. Добавь компонент `Projectile2D` (скрипт).
10. Проверь, что размер projectile визуально маленький (Scale около `0.2..0.4` при необходимости).
11. Открой `Project -> Assets/Prefabs`.
12. Перетащи объект `Projectile` из `Hierarchy` в `Assets/Prefabs`, чтобы создать `Projectile.prefab`.
13. После создания prefab удали `Projectile` из `Hierarchy` (в сцене он не нужен как постоянный объект).
14. Выдели объект `Player` в `Hierarchy`.
15. Нажми `Add Component` и добавь `PlayerAttack`.
16. На `Player` создай дочерний объект:
    `Right Click on Player -> Create Empty`
17. Переименуй дочерний объект в `FirePoint`.
18. Установи `FirePoint` чуть перед игроком (например `Local Position X = 0.4`, `Y = 0`, `Z = 0`).
19. Вернись к компоненту `PlayerAttack` на `Player`.
20. Перетащи `Projectile.prefab` в поле `Projectile Prefab`.
21. Перетащи `FirePoint` из `Hierarchy` в поле `Fire Point`.
22. Убедись, что `PlayerController` на игроке присутствует (обычно уже есть).
23. (Подготовка под урон) для врагов выставляй `Tag = Enemy` на их корневом объекте.
24. Если у врага есть скрипт HP, добавь в нём метод `TakeDamage(int damage)` (именно с таким названием), чтобы projectile наносил урон через `SendMessage`.

## Acceptance Checks (Play Mode)

1. Нажми `Play`.
2. Кликни мышью (`Mouse0`): появляются projectile и летят в сторону курсора.
3. Нажми `Space`: тоже должен происходить выстрел.
4. Временно убери тег `MainCamera` у камеры:
   выстрел должен идти в `LastMoveDirection` (направление последнего движения игрока).
5. Проверь, что projectile исчезает по времени (`lifetime`) и не остаётся в сцене бесконечно.
6. При попадании в объект с тегом `Enemy` projectile исчезает сразу.

---

# Enemy Chase + HP + Contact Damage + Score (CP-05)

Добавлен базовый враг с преследованием игрока, здоровьем, контактным уроном и начислением очков за смерть.

## Скрипты (CP-05)

- `Assets/Scripts/Enemies/EnemyChaseAI.cs`: движение к игроку через `Rigidbody2D.MovePosition`.
- `Assets/Scripts/Enemies/EnemyHealth.cs`: HP врага, `TakeDamage(int)`, защита от повторной смерти (`isDead`), `GameRoot.AddScore(...)` при смерти.
- `Assets/Scripts/Enemies/EnemyContactDamage.cs`: контактный урон игроку с cooldown (по умолчанию `0.5s`).
- `Assets/Scripts/Projectile2D.cs`: projectile теперь вызывает `EnemyHealth.TakeDamage(...)` напрямую.
- `Assets/Scripts/Core/GameRoot.cs`: добавлен `GameRoot.Instance` для безопасного доступа из систем врага.

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Game`.
2. В `Hierarchy` нажми `Right Click -> 2D Object -> Sprites -> Square` (или любой sprite для врага).
3. Переименуй объект в `Enemy`.
4. Подгони размер врага через `Transform -> Scale` (например `0.8, 0.8, 1`).
5. С выбранным `Enemy` нажми `Add Component`.
6. Добавь `Rigidbody2D`.
7. В `Rigidbody2D` выставь:
   `Body Type = Dynamic`
   `Gravity Scale = 0`
   `Constraints -> Freeze Rotation Z = On`
8. Нажми `Add Component` и добавь `BoxCollider2D` или `CircleCollider2D`.
9. Убедись, что у collider **выключен** `Is Trigger` (нужен физический контакт).
10. Нажми `Add Component` и добавь `EnemyChaseAI`.
11. Нажми `Add Component` и добавь `EnemyHealth`.
12. Нажми `Add Component` и добавь `EnemyContactDamage`.
13. Вверху Inspector у объекта `Enemy` открой `Tag` и выбери `Enemy`.
14. Если тега `Enemy` нет:
    `Tag -> Add Tag... -> + -> Enemy`, затем вернись к объекту и назначь `Tag = Enemy`.
15. Проверь настройки `EnemyContactDamage`:
    `Damage Per Hit` (например `10`)
    `Damage Cooldown = 0.5`
16. Проверь настройки `EnemyHealth`:
    `Max Hp` (например `3`)
    `Score On Death` (например `10`)
17. Перетащи объект `Enemy` из `Hierarchy` в `Assets/Prefabs`, чтобы создать `Enemy.prefab`.
18. Оставь в сцене `Game` одного врага рядом с игроком для теста.
19. Выдели `Player` и проверь, что у него есть `Collider2D` и `Rigidbody2D`.
20. Проверь, что на `Player` есть `PlayerHealth`.
21. (Рекомендуется) Назначь игроку `Tag = Player`, чтобы поиск цели у врага был мгновенный.
22. Убедись, что у projectile prefab есть `Collider2D` c `Is Trigger = On` и скрипт `Projectile2D`.
23. Нажми `Play`.

## Acceptance Checks (Play Mode)

1. Враг начинает двигаться в сторону игрока.
2. При попадании projectile враг теряет HP и исчезает после обнуления HP.
3. При касании врага игрок получает урон (не каждый physics tick, а с шагом cooldown).
4. После смерти врага увеличивается `Score` в `GameRoot`.

## Notes

- Если игрок не найден в сцене, `EnemyChaseAI` не падает в ошибку и враг просто стоит.
- Повторная смерть врага заблокирована `isDead`.
- Контактный урон по умолчанию с cooldown `0.5s` (по заданному assumption).

---

# Wave Manager + Win Condition (CP-06)

Реализованы волны спавна врагов с победой после зачистки последней волны.

## Скрипты (CP-06)

- `Assets/Scripts/Core/WaveManager.cs`: спавн врагов волнами по таймеру в `Spawn Points`, учёт `aliveEnemies`, вызов `EndGame(true)` после последней волны и полной зачистки.
- `Assets/Scripts/Core/GameRoot.cs`: добавлен `SetCurrentWave(int)`, чтобы фиксировать номер текущей волны для `RunResultStore`.
- `Assets/Scripts/Core/RunResultStore.cs`: добавлен `HasResult` (флаг валидности результата забега).
- `Assets/Scripts/Enemies/EnemyHealth.cs`: событие `Died`, чтобы `WaveManager` корректно уменьшал `aliveEnemies`.

## Assumption

- По умолчанию используется 4 волны.
- Между волнами короткая пауза `2` секунды.

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Game`.
2. В `Hierarchy` нажми `Right Click -> Create Empty`.
3. Переименуй объект в `WaveManager`.
4. С выбранным `WaveManager` нажми `Add Component`.
5. Добавь скрипт `WaveManager`.
6. Создай 4 точки спавна:
   `Right Click -> Create Empty` и назови по очереди:
   `SpawnPoint_1`, `SpawnPoint_2`, `SpawnPoint_3`, `SpawnPoint_4`.
7. Расставь `SpawnPoint_1..4` по краям арены (верх/низ/лево/право).
8. Выдели объект `WaveManager`.
9. В компоненте `WaveManager` найди поле `Spawn Points`.
10. Установи размер массива `Spawn Points` в `4`.
11. Перетащи из `Hierarchy`:
    `SpawnPoint_1` в `Element 0`,
    `SpawnPoint_2` в `Element 1`,
    `SpawnPoint_3` в `Element 2`,
    `SpawnPoint_4` в `Element 3`.
12. В поле `Enemy Prefab` перетащи `Assets/Prefabs/Enemy.prefab`.
13. В поле `Waves` проверь размер массива `4`.
14. Настрой волны, например:
    `Wave 1: count=3`,
    `Wave 2: count=5`,
    `Wave 3: count=7`,
    `Wave 4: count=9`.
15. Для всех волн задай `spawnInterval` (например `0.7`).
16. Поле `Inter Wave Delay` оставь `2`.
17. Выдели `GameRoot`.
18. Убедись, что поле `Wave Manager` ссылается на объект `WaveManager` (если пусто, перетащи вручную).
19. Сохрани сцену `Game` (`Ctrl/Cmd + S`).
20. Нажми `Play`.

## Acceptance Checks (Play Mode)

1. Враги появляются волнами и спавнятся только в `SpawnPoint_1..4`.
2. Между волнами есть короткая пауза, игра не зависает.
3. После убийства всех врагов последней волны вызывается `EndGame(true)`.
4. Происходит переход на сцену `Results` с результатом `Win`.

---

# HUD Events (CP-08)

Реализован событийный HUD без обновления текста каждый кадр.

## Скрипты (CP-08)

- `Assets/Scripts/Core/GameRoot.cs`
  - Добавлены события:
    - `ScoreChanged`
    - `WaveChanged`
    - `TimeChanged` (дискретный тик, по умолчанию раз в `0.5` сек, диапазон `0.2..1`).
  - Добавлены свойства для чтения HUD:
    - `CurrentWave`
    - `RunTimeSeconds`
- `Assets/Scripts/Core/WaveManager.cs`
  - Добавлены `CurrentWave` и событие `WaveChanged`.
  - Событие вызывается при старте каждой волны.
- `Assets/Scripts/Core/PlayerHealth.cs`
  - Используется событие `HealthChanged` для HUD.
- `Assets/Scripts/UI/HUDController.cs`
  - Подписка на события `HP/Wave/Time/Score`.
  - Безопасные `null`-проверки.
  - Корректная отписка в `OnDisable` и `OnDestroy`.
  - Формат текста:
    - `HP: ...`
    - `Wave: ...`
    - `Time: ...`
    - `Score: ...`

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Game`.
2. В `Hierarchy` нажми `Right Click -> UI -> Canvas`.
3. Убедись, что у Canvas `Render Mode = Screen Space - Overlay`.
4. Выдели `Canvas`.
5. Нажми `Right Click -> UI -> Text - TextMeshPro`.
6. Если Unity попросит импорт TMP Essentials: нажми `Import TMP Essentials`.
7. Переименуй текст в `HPText`.
8. Повтори создание ещё 3 раза и назови объекты:
   `WaveText`, `TimeText`, `ScoreText`.
9. Расположи тексты как удобно:
   - `HPText`: верхний левый угол.
   - `WaveText`: верхний центр.
   - `TimeText`: верхний правый угол.
   - `ScoreText`: любой свободный верхний угол/центр (например рядом с `TimeText`).
10. В `Hierarchy` нажми `Right Click -> Create Empty`.
11. Переименуй объект в `HUD`.
12. Выдели `HUD` и нажми `Add Component`.
13. Добавь `HUDController`.
14. В Inspector у `HUDController` назначь ссылки:
   - `Hp Text` -> `HPText`
   - `Wave Text` -> `WaveText`
   - `Time Text` -> `TimeText`
   - `Score Text` -> `ScoreText`
15. (Рекомендуется) Назначь источники данных явно:
   - `Game Root` -> объект `GameRoot`
   - `Wave Manager` -> объект `WaveManager`
   - `Player Health` -> объект `Player` (компонент `PlayerHealth`)
16. Сохрани сцену (`Ctrl/Cmd + S`).
17. Нажми `Play`.

## Acceptance Checks (Play Mode)

1. Сразу после старта видны стартовые значения HUD:
   - `HP:`
   - `Wave:`
   - `Time:`
   - `Score:`
2. При получении урона игроком значение `HP` обновляется.
3. При убийстве врага обновляется `Score`.
4. При старте новой волны обновляется `Wave`.
5. `Time` растёт дискретно по тикам (не через спам обновления текста каждый кадр).

---

# Results Screen (CP-09)

Реализован экран результатов с показом итога забега и кнопками перехода.

## Скрипты (CP-09)

- `Assets/Scripts/UI/ResultsUIController.cs`
  - Читает `RunResultStore` и показывает:
    - заголовок `YOU WIN` / `YOU LOSE`;
    - `Score`;
    - `Wave` (опционально);
    - `Time` (опционально).
  - Безопасно работает при пустом `RunResultStore` (`HasResult = false`): показывает дефолтные значения.
  - Методы для UI кнопок:
    - `OnRestartClicked()` -> загружает сцену `Game`.
    - `OnMainMenuClicked()` -> загружает сцену `MainMenu`.
- `Assets/Scripts/Core/RunResultStore.cs`
  - Добавлен `TryGetResult(...)` для безопасного чтения с дефолтами.
- `Assets/Scripts/Core/SceneLoader.cs`
  - Имена сцен вынесены в константы:
    - `MainMenuSceneName`
    - `GameSceneName`
    - `ResultsSceneName`
  - Добавлены методы:
    - `LoadGameScene()`
    - `LoadMainMenuScene()`
    - `LoadResultsScene()`

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Results`.
2. В `Hierarchy` нажми `Right Click -> UI -> Canvas`.
3. Убедись, что у `Canvas` стоит `Render Mode = Screen Space - Overlay`.
4. На `Canvas` создай заголовок:
   `Right Click on Canvas -> UI -> Text - TextMeshPro`.
5. Если Unity спросит про TMP essentials, нажми `Import TMP Essentials`.
6. Переименуй текст в `ResultTitleText`.
7. Увеличь размер шрифта (например `60..90`) и поставь по центру вверху.
8. Создай второй текст:
   `Right Click on Canvas -> UI -> Text - TextMeshPro`.
9. Переименуй его в `FinalScoreText`.
10. Поставь его под заголовком.
11. Создай третий текст (опционально):
    `Right Click on Canvas -> UI -> Text - TextMeshPro`.
12. Переименуй его в `FinalWaveText`.
13. Поставь его ниже `FinalScoreText`.
14. Создай четвёртый текст (опционально):
    `Right Click on Canvas -> UI -> Text - TextMeshPro`.
15. Переименуй его в `FinalTimeText`.
16. Поставь его ниже `FinalWaveText`.
17. Создай кнопку рестарта:
    `Right Click on Canvas -> UI -> Button - TextMeshPro`.
18. Переименуй кнопку в `RestartButton`.
19. Измени надпись кнопки на `Restart`.
20. Поставь кнопку в нижней части экрана слева или по центру.
21. Создай вторую кнопку:
    `Right Click on Canvas -> UI -> Button - TextMeshPro`.
22. Переименуй кнопку в `MainMenuButton`.
23. Измени надпись кнопки на `Main Menu`.
24. Поставь вторую кнопку рядом с `RestartButton`.
25. В `Hierarchy` нажми `Right Click -> Create Empty`.
26. Переименуй объект в `ResultsUI`.
27. Выдели `ResultsUI`, нажми `Add Component`.
28. Добавь компонент `ResultsUIController`.
29. В Inspector у `ResultsUIController` назначь ссылки:
    - `Result Title Text` -> `ResultTitleText`
    - `Final Score Text` -> `FinalScoreText`
    - `Final Wave Text` -> `FinalWaveText` (если используешь)
    - `Final Time Text` -> `FinalTimeText` (если используешь)
    - `Restart Button` -> `RestartButton`
    - `Main Menu Button` -> `MainMenuButton`
30. (Опционально) Если в сцене есть объект с `SceneLoader`, перетащи его в поле `Scene Loader`.
31. Выдели `RestartButton`.
32. В компоненте `Button` найди блок `On Click ()`.
33. Нажми `+`.
34. Перетащи объект `ResultsUI` в появившийся объект-слот.
35. В выпадающем списке выбери:
    `ResultsUIController -> OnRestartClicked()`.
36. Выдели `MainMenuButton`.
37. В блоке `On Click ()` нажми `+`.
38. Перетащи объект `ResultsUI` в объект-слот.
39. В выпадающем списке выбери:
    `ResultsUIController -> OnMainMenuClicked()`.
40. Сохрани сцену (`Ctrl/Cmd + S`).
41. Проверь `File -> Build Settings`: сцены `MainMenu`, `Game`, `Results` должны быть в списке.
42. Нажми `Play`.

## Acceptance Checks (Play Mode)

1. Заверши забег победой: на `Results` должен быть заголовок `YOU WIN`.
2. Заверши забег поражением: на `Results` должен быть заголовок `YOU LOSE`.
3. Проверь, что `Score` показывает число больше `0`, если были убийства.
4. Нажми `Restart` -> открывается сцена `Game`.
5. Нажми `Main Menu` -> открывается сцена `MainMenu`.

---

# Main Menu: Start / Quit + Game Transition (CP-10)

Реализовано простое Lite-меню без подменю: кнопки `Start` и `Quit`.

## Скрипты (CP-10)

- `Assets/Scripts/UI/MainMenuController.cs`
  - `OnStartClicked()`:
    - делает `RunResultStore.Reset()` перед новой игрой;
    - проверяет, что имя сцены не пустое;
    - загружает сцену `Game` через `SceneLoader`.
  - `OnQuitClicked()`:
    - вызывает `Application.Quit()`;
    - в Editor пишет `Debug.Log`, что был вызван quit.

## Assumption

Lite-версия `MainMenu` без настроек и подменю.

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `MainMenu`.
2. Если сцены нет: создай её в `Assets/Scenes` с именем `MainMenu` и открой.
3. В `Hierarchy` нажми `Right Click -> UI -> Canvas`.
4. Если Unity предложит создать `EventSystem`, нажми `Create`.
5. Убедись, что в `Hierarchy` есть `Canvas` и `EventSystem`.
6. В `Hierarchy` выбери `Canvas`.
7. Нажми `Right Click on Canvas -> UI -> Text - TextMeshPro`.
8. Если TMP попросит импорт essentials, нажми `Import TMP Essentials`.
9. Переименуй текст в `TitleText`.
10. В Inspector у `TitleText` установи текст: `Arena Survivor: Lite`.
11. Настрой позицию заголовка в верхней части экрана (RectTransform, Anchor Top Center).
12. На `Canvas` нажми `Right Click -> UI -> Button - TextMeshPro`.
13. Переименуй кнопку в `StartButton`.
14. У дочернего текста кнопки поставь текст: `Start`.
15. Расположи `StartButton` примерно по центру.
16. Сдублируй `StartButton` (`Ctrl + D` / `Cmd + D`).
17. Переименуй копию в `QuitButton`.
18. У дочернего текста копии поставь текст: `Quit`.
19. Смести `QuitButton` ниже `StartButton`.
20. В `Hierarchy` нажми `Right Click -> Create Empty`.
21. Переименуй объект в `MainMenuUI`.
22. Выдели `MainMenuUI` и нажми `Add Component`.
23. Добавь компонент `MainMenuController`.
24. (Опционально) Если в сцене есть объект со `SceneLoader`, перетащи его в поле `Scene Loader` у `MainMenuController`.
25. Убедись, что поле `Game Scene Name` у `MainMenuController` равно `Game`.
26. Выдели `StartButton`.
27. В компоненте `Button` в секции `On Click()` нажми `+`.
28. Перетащи `MainMenuUI` в слот объекта нового события.
29. В выпадающем списке функции выбери: `MainMenuController -> OnStartClicked()`.
30. Выдели `QuitButton`.
31. В компоненте `Button` в секции `On Click()` нажми `+`.
32. Перетащи `MainMenuUI` в слот объекта нового события.
33. В выпадающем списке функции выбери: `MainMenuController -> OnQuitClicked()`.
34. Открой `File -> Build Settings`.
35. В `Scenes In Build` проверь, что `MainMenu` добавлена и стоит первой (`index 0`).
36. Проверь, что сцена `Game` тоже добавлена в `Scenes In Build`.
37. Сохрани сцену `MainMenu` (`Ctrl + S` / `Cmd + S`).

## Acceptance Checks (Play Mode)

1. Нажми `Play` из сцены `MainMenu`.
2. Убедись, что отображаются заголовок `Arena Survivor: Lite`, кнопки `Start` и `Quit`.
3. Нажми `Start` -> должна загрузиться сцена `Game`.
4. Останови Play Mode и снова запусти из `MainMenu`.
5. Нажми `Quit` в Editor -> в Console должен появиться лог:
   `MainMenuController.OnQuitClicked: Application.Quit() called in Editor.`

---

# Mecanim Player States + Driver (CP-11)

Добавлена базовая связка анимаций игрока: `Idle` / `Run` / `Attack` / `Die` через параметры Animator и код-драйвер.

## Скрипты (CP-11)

- `Assets/Scripts/PlayerAnimatorDriver.cs`
  - Берёт ссылки на `Animator`, `PlayerController`, `PlayerHealth`, `PlayerAttack`.
  - Обновляет `Speed` по фактической скорости движения (`PlayerController.CurrentSpeed`).
  - Дёргает trigger `Attack` только при успешном `TryFire()` (через событие успешного выстрела).
  - Ставит bool `IsDead` при смерти игрока.
  - Если `Animator` отсутствует, не падает, а выводит `warning`.
- `Assets/Scripts/PlayerController.cs`
  - Добавлен `CurrentSpeed` с расчётом по фактическому смещению `Rigidbody2D`.
- `Assets/Scripts/PlayerAttack.cs`
  - `TryFire()` теперь возвращает `bool` успеха.
  - Добавлено событие `Fired` только при успешном выстреле.
  - После смерти (`PlayerHealth.IsDead`) выстрелы блокируются.
- `Assets/Scripts/Core/PlayerHealth.cs`
  - Добавлено публичное свойство `IsDead`.

## Assumption

Используются placeholder clips для 4 состояний (`Idle`, `Run`, `Attack`, `Die`), даже если это временные дубликаты.

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Game`.
2. В `Hierarchy` выбери объект `Player`.
3. Нажми `Add Component`.
4. Добавь компонент `Animator`.
5. В `Project` открой папку `Assets/Animations`.
6. Если папки нет: `Right Click -> Create -> Folder`, назови `Animations`.
7. В папке `Assets/Animations` нажми `Right Click -> Create -> Animator Controller`.
8. Назови контроллер `Player.controller`.
9. Дважды кликни `Player.controller`, чтобы открыть окно `Animator`.
10. В окне `Animator` создай state `Idle`:
    `Right Click -> Create State -> Empty`, переименуй в `Idle`.
11. Повтори и создай `Run`.
12. Повтори и создай `Attack`.
13. Повтори и создай `Die`.
14. Назначь клипы в состояния `Idle/Run/Attack/Die` (placeholder клипы тоже подходят).
15. Сделай `Idle` состоянием по умолчанию:
    `Right Click on Idle -> Set as Layer Default State`.
16. В `Animator` открой вкладку `Parameters`.
17. Нажми `+ -> Float`, создай параметр `Speed`.
18. Нажми `+ -> Trigger`, создай параметр `Attack`.
19. Нажми `+ -> Bool`, создай параметр `IsDead`.
20. Создай переход `Idle -> Run`:
    `Right Click Idle -> Make Transition -> Run`.
21. Выдели переход `Idle -> Run`, в `Inspector` добавь Condition:
    `Speed` `Greater` `0.1`.
22. Создай переход `Run -> Idle`:
    `Right Click Run -> Make Transition -> Idle`.
23. Выдели переход `Run -> Idle`, в `Inspector` добавь Condition:
    `Speed` `Less` `0.1`.
24. Создай переход `Any State -> Attack`:
    `Right Click Any State -> Make Transition -> Attack`.
25. Выдели этот переход и добавь Condition:
    `Attack` (trigger).
26. Создай переход `Any State -> Die`:
    `Right Click Any State -> Make Transition -> Die`.
27. Выдели этот переход и добавь Condition:
    `IsDead` `true`.
28. Выдели объект `Player` в `Hierarchy`.
29. В компоненте `Animator` перетащи `Assets/Animations/Player.controller` в поле `Controller`.
30. На объекте `Player` нажми `Add Component`.
31. Добавь компонент `PlayerAnimatorDriver`.
32. В `PlayerAnimatorDriver` назначь ссылки:
    - `Player Animator` -> компонент `Animator` с `Player.controller`
    - `Player Controller` -> компонент `PlayerController` на `Player`
    - `Player Health` -> компонент `PlayerHealth` на `Player`
    - `Player Attack` -> компонент `PlayerAttack` на `Player`
33. Сохрани сцену `Game` (`Ctrl/Cmd + S`).
34. Нажми `Play`.

## Acceptance Checks (Play Mode)

1. Начни движение игроком (`WASD`/стрелки): должен включаться `Run`.
2. Остановись: должен вернуться `Idle`.
3. Нажми выстрел (`Mouse0` или `Space`): должен сработать `Attack`.
4. Получи летальный урон игроком: должен включиться `Die`.
5. После смерти проверь, что игра уходит в `Results`.

---

# Audio SFX Wiring + Final Build Readiness (CP-12)

Добавлена базовая SFX-система и связка аудио-событий для `shoot / hit / die`.

## Baseline Coverage (CP-12)

- `Assets/Scripts/Audio/AudioManager.cs`
  - `PlaySfx(SfxType type)` для воспроизведения SFX через `AudioSource`.
  - Поддержка типов: `Shoot`, `Hit`, `Die`, `UIClick`.
  - Если `AudioSource` отсутствует: `Debug.LogError` и безопасный выход без падения.
  - Если клип не назначен: `Debug.LogWarning` и безопасный выход без падения.
- `Assets/Scripts/PlayerAttack.cs`
  - `Shoot` SFX при успешном выстреле.
- `Assets/Scripts/Enemies/EnemyHealth.cs`
  - `Hit` SFX при получении урона.
  - `Die` SFX при смерти врага.
- `Assets/Scripts/Core/PlayerHealth.cs`
  - `Die` SFX при смерти игрока.
- `Assets/Scripts/UI/MainMenuController.cs` (опционально)
  - `UIClick` SFX на `Start` и `Quit`.

## Unity Editor Steps (ультра-пошагово для новичка)

### A. Настройка AudioManager в сцене Game

1. Открой сцену `Game`.
2. В `Hierarchy` нажми `Right Click -> Create Empty`.
3. Переименуй объект в `AudioManager`.
4. Выдели объект `AudioManager`.
5. Нажми `Add Component`.
6. Добавь скрипт `AudioManager`.
7. Нажми `Add Component` ещё раз.
8. Добавь компонент `AudioSource`.
9. В `AudioSource` выключи `Play On Awake`.
10. (Рекомендуется) `Spatial Blend = 0` (2D звук).
11. В `AudioManager` найди поле `Sfx Entries`.
12. Поставь `Size = 3` минимум.
13. Элемент `0`: `Type = Shoot`, `Clip = Assets/Audio/<shoot_clip>`, `Volume = 1`.
14. Элемент `1`: `Type = Hit`, `Clip = Assets/Audio/<hit_clip>`, `Volume = 1`.
15. Элемент `2`: `Type = Die`, `Clip = Assets/Audio/<die_clip>`, `Volume = 1`.
16. Если нужен UI клик: увеличь `Size` до `4`.
17. Новый элемент: `Type = UIClick`, `Clip = Assets/Audio/<ui_click_clip>`, `Volume = 1`.
18. Убедись, что поле `Sfx Source` указывает на этот же `AudioSource`.

### B. Доступ к AudioManager в MainMenu и Results

1. Открой сцену `MainMenu`.
2. Проверь, есть ли в `Hierarchy` объект `AudioManager`.
3. Если нет, создай `Create Empty -> AudioManager`.
4. Добавь компоненты `AudioManager` и `AudioSource`.
5. В `AudioSource` выключи `Play On Awake`.
6. Назначь минимум клип `UIClick` (и при желании остальные).
7. Открой сцену `Results`.
8. Повтори те же шаги при необходимости.
9. Альтернатива: использовать один `AudioManager` с `Dont Destroy On Load`.
10. Если используешь один глобальный экземпляр, не оставляй дубликаты в каждой сцене.

### C. Быстрые визуальные проверки (во избежание ложных проблем)

1. Выдели `Main Camera` в каждой сцене.
2. Проверь `Projection = Orthographic`.
3. Выдели спрайты игрока/врагов/пуль.
4. Проверь, что материал спрайтов `Sprites-Default`.
5. Если есть розовые объекты, заново назначь корректный материал спрайта.

## Final Build Checklist

1. В `File -> Build Settings` добавлены сцены:
   - `MainMenu`
   - `Game`
   - `Results`
2. Порядок сцен корректный (`MainMenu` первой).
3. Во всех нужных сценах есть доступ к `AudioManager`.
4. У каждого `AudioManager` есть `AudioSource` и выключен `Play On Awake`.
5. Назначены минимум 3 SFX-клипа: `Shoot`, `Hit`, `Die`.
6. В Console нет критических ошибок (`NullReferenceException`, Missing Script).
7. Сохранены все изменённые сцены перед билдом.

## Smoke Test / Verification List

1. `MainMenu -> Start` запускает `Game` без критических ошибок в Console.
2. При выстреле (`Mouse0`/`Space`) слышен `Shoot SFX`.
3. При попадании по врагу слышен `Hit SFX`.
4. При смерти врага слышен `Die SFX`.
5. При смерти игрока слышен `Die SFX`.
6. Полный цикл работает:
   `MainMenu -> Game -> Win/Lose -> Results -> Restart/MainMenu`.
7. `Restart` из `Results` корректно возвращает в `Game`.
8. `MainMenu` из `Results` корректно возвращает в `MainMenu`.
9. (Опционально) Кнопки меню издают `UIClick SFX`.
10. После полного прогона нет новых критических ошибок в Console.

---

# Visual Readability Pass (CP-15)

Цель этого шага: улучшить визуальную читаемость без изменения базовых механик.

## Добавленные/обновлённые скрипты (CP-15)

- `Assets/Scripts/SceneVisualBootstrap.cs`
- `Assets/Scripts/PlayerSpriteFacing.cs`
- `Assets/Scripts/SpriteHitFlash.cs`
- `Assets/Scripts/ProjectileVisual.cs`
- `Assets/Scripts/CameraFollow2D.cs`
- `Assets/Scripts/Enemies/EnemyHealth.cs` (flash + fade на смерть)
- `Assets/Scripts/Core/PlayerHealth.cs` (flash на урон)
- `Assets/Scripts/PlayerAttack.cs` (инициализация `ProjectileVisual`)

## Quick Tuning Values

- `Main Camera -> Orthographic Size`: `5..7` (старт: `6`)
- `SceneVisualBootstrap -> Background Color`: тёмный ненасыщенный (старт: `0.09 / 0.11 / 0.14`)
- `SpriteHitFlash -> Flash Duration`: `0.1`
- `SpriteHitFlash -> Flash Color`: светло-красный (пример: `#FF8C8C`)
- `EnemyHealth -> Death Fade Duration`: `0.3` (допуск: `0.2..0.4`)
- `ProjectileVisual -> Scale Pulse Amount`: `0.05..0.1` (старт: `0.07`)
- `ProjectileVisual -> Spin Speed`: низкий, около `40..70` (старт: `60`)
- `CameraFollow2D -> Follow Lerp Speed`: `6..10` (старт: `8`)

## Unity Editor Steps (ультра-пошагово для новичка)

1. Открой сцену `Game`.
2. Открой `Edit -> Project Settings -> Tags and Layers`.
3. В `Sorting Layers` создай слои по порядку:
   `Background`, `Units`, `Projectiles`, `FX`, `UI`.
4. В `Hierarchy` создай `2D Object -> Sprites -> Square`.
5. Переименуй в `Background`.
6. Растяни `Background` так, чтобы он закрывал всю арену.
7. У объекта `Background` в `SpriteRenderer` поставь `Sorting Layer = Background`.
8. Выдели `Player` и поставь `SpriteRenderer -> Sorting Layer = Units`.
9. Выдели `Enemy` (или открой `Enemy.prefab`) и поставь `SpriteRenderer -> Sorting Layer = Units`.
10. Открой `Assets/Prefabs/Projectile.prefab`.
11. В `SpriteRenderer` projectile поставь `Sorting Layer = Projectiles`.
12. Выдели `Player` в сцене.
13. Нажми `Add Component` и добавь `PlayerSpriteFacing`.
14. В `PlayerSpriteFacing` назначь `Target Renderer` на `Player` SpriteRenderer.
15. На `Player` нажми `Add Component` и добавь `SpriteHitFlash`.
16. В `SpriteHitFlash` назначь `Target Renderer` на `Player` SpriteRenderer.
17. В `SpriteHitFlash` у `Player` выставь:
    `Flash Duration = 0.1`, `Flash Color = light red`.
18. Открой `Assets/Prefabs/Enemy.prefab`.
19. Добавь компонент `SpriteHitFlash` (если его нет).
20. Назначь в нём `Target Renderer` на SpriteRenderer врага.
21. Выставь `Flash Duration = 0.1`, `Flash Color = light red`.
22. На этом же `Enemy.prefab` в `EnemyHealth` проверь:
    `Sprite Renderer` назначен, `Death Fade Duration` примерно `0.3`.
23. Сохрани `Enemy.prefab`.
24. Открой `Assets/Prefabs/Projectile.prefab`.
25. Добавь компонент `ProjectileVisual`.
26. Выставь у `ProjectileVisual`:
    `Scale Pulse Amount = 0.05..0.1`, `Spin Speed = low`.
27. Сохрани `Projectile.prefab`.
28. Выдели `Main Camera`.
29. Добавь компонент `CameraFollow2D`.
30. В `CameraFollow2D` перетащи `Player` в поле `Target`.
31. Проверь `Main Camera -> Projection = Orthographic`.
32. Проверь `Main Camera -> Size` около `5..7`.
33. В `Hierarchy` нажми `Right Click -> Create Empty`.
34. Переименуй объект в `SceneVisualBootstrap`.
35. Добавь компонент `SceneVisualBootstrap`.
36. (Опционально) В `SceneVisualBootstrap` назначь `Target Camera = Main Camera`.
37. У `Player`, `Enemy`, `Projectile`, `Background` проверь:
    `SpriteRenderer -> Material = Sprites-Default`.
38. Убедись, что в сцене нет розовых материалов/спрайтов.
39. Нажми `Play`.
40. Вручную подстрой цвета, `Orthographic Size`, и параметры `CameraFollow2D/ProjectileVisual`, чтобы игрок и враги чётко различались.

## Acceptance Checks (Play Mode)

1. Игрок визуально смотрит в сторону движения/выстрела (`flipX` работает).
2. При уроне у `Player` и `Enemy` виден короткий hit flash.
3. При смерти враг плавно исчезает (fade), затем удаляется.
4. Projectile выглядит живее (лёгкий pulse/spin), но не мешает геймплею.
5. Камера мягко следует за игроком без резких рывков.
6. Базовые механики не ломаются: движение, атака, волны, win/lose, UI, audio.
7. В Console нет спама `NullReferenceException` и критических ошибок.

# CliInterface - каталог персонажів через CLI-фасад

![build](https://github.com/Markentyy/CliInterface-ProjectOOP/actions/workflows/build.yml/badge.svg)
![dotnet](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![csharp](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)
![license](https://img.shields.io/badge/license-MIT-green)

Навчальний проєкт за ТЗ "Командний інтерфейc додатку"

## Зміст

- [Можливості](#можливості)
- [Технології](#технології)
- [Структура](#структура)
- [Швидкий старт](#швидкий-старт)
- [Команди](#команди)
- [Сумісність з версіями .NET](#сумісність-з-версіями-net)
- [Дизайн-документ](#дизайн-документ)
- [Ролі класів](#ролі-класів)
- [Автор](#автор)
- [Ліцензія](#ліцензія)

## Можливості

* інтерпретатор з рядком `>`, лапки групують слова з пробілами,
  `help` показує команди, згруповані за стратегіями;
* каталог персонажів: створення діалогом, спорядження, здібності,
  посилання на графіку у кожного персонажа;
* автоматичне збереження після кожної успішної мутації та завантаження
  при старті (`--data <шлях>`, файл за замовчуванням `data/world.json`);
* команди зібрані у три стратегії: інформаційні, трансформативні
  (автозбереження), інфраструктурні (дані і довідка);
* короткий вигляд - вирівняна таблиця з HP-баром, детальний - картка
  персонажа з посиланням на графіку.

## Технології

| Технологія | Версія / примітка |
|---|---|
| C# | 12 |
| .NET (цільова платформа) | 8.0 (`net8.0`, `RollForward LatestMajor`) |
| Збірка і тести | xUnit, `TreatWarningsAsErrors` + XML-документація |
| Дані | `System.Text.Json` |

## Структура

```
src/
  Core/     домен: персонажі, предмети, здібності, реєстри, бій
  Data/     збереження: IWorldStore + JSON-сховище
  Cli/      фасад, REPL, парсер, рендери, стратегії і команди
  App/      точка входу (Program.cs)
  Tests.*   юніт-тести по шарах
```

Залежності: `App -> {Cli -> {Core, Data}, Data -> Core}`.
Шар `Core` не знає ні про консоль, ні про збереження.

## Швидкий старт

Потрібен SDK 8.0 (або новіший - з увімкненим RollForward).

```bash
dotnet build src/App/App.csproj

# інтерактивний запуск (дані у data/world.json)
dotnet run --project src/App/App.csproj --no-build

# свій файл даних
dotnet run --project src/App/App.csproj --no-build -- --data /tmp/demo.json

# прогін демо-сценарію
dotnet run --project src/App/App.csproj --no-build -- --data /tmp/demo.json < demo/demo.txt

# тести
dotnet test
```

## Команди

| Команда | Приклад | Стратегія | Опис |
|---|---|---|---|
| `ls` | `ls` | Info | таблиця всіх персонажів (HP-бар, ATK/ARM, кількість предметів і здібностей) |
| `info <id\|name>` | `info Mage` | Info | картка: здоров'я, бонуси, посилання на графіку, інвентар, книга здібностей |
| `create <char\|item\|ability>` | `create char` | Transform | діалог створення з усіма полями; порожній id - авто `char-N` |
| `add --char_id <id\|name> --id <id\|name>` | `add --char_id Mage --id Sword` | Transform | одягнути предмет або вивчити здібність; після успіху - збереження |
| `save` | `save` | Infra | зберегти каталог у файл даних |
| `load` | `load` | Infra | перечитати файл даних (скидає поточний стан) |
| `help` | `help` | Infra | список команд, згрупованих за стратегіями |
| `exit` | `exit` | - | вийти з інтерпретатора |

Приклад сесії:

```
> create char
Name: Gatekeeper
MaxHealth [1-10000]: 100
BaseArmor [0-1000]: 5
BaseAttack [0-10000]: 10
Graphics URL (empty to skip): https://example.com/gate.png
Id (empty for auto):
Created character 'Gatekeeper' [char-1] (100 HP, 5 ARM, 10 ATK).

> ls
Id        Name        Health              ATK  ARM  Items  Abilities
--------  ----------  ------------------  ---  ---  -----  ---------
[char-1]  Gatekeeper  ########## 100/100  10   5    0      0

> info Gatekeeper
[char-1] Gatekeeper
  Health: ########## 100/100
  Attack: 10 base, 10 total
  Armor:  5 base, 5 total
  Graphics: https://example.com/gate.png
  Items: -
  Abilities: -
```

## Сумісність з версіями .NET

Цільова платформа проєкту - `net8.0` з `<RollForward>LatestMajor</RollForward>`,
тому збирається і під SDK 9/10.

## Дизайн-документ

Детальний опис інтерфейсу і діаграми: [docs/design.md](docs/design.md).

## Ролі класів

| Шар | Клас | Роль |
|---|---|---|
| Core | `Character`, `Equipment`, `Ability` | доменні об'єкти каталогу |
| Core | `CombatResolver` | чиста математика шкоди і лікування |
| Core | `Registry<T>` | іменоване сховище з авто-ідентифікаторами |
| Core | `CharacterWorld` | агрегат: реєстри + книга здібностей |
| Data | `IWorldStore` | порт збереження |
| Data | `JsonWorldStore` | атомарний JSON-файл (запис через `.tmp`) |
| Cli | `Cli` | фасад: `useStrategy`, `execCommand`, `display` |
| Cli | `LineParser`, `ParsedCommand` | розбір рядка: слова в лапках, `--опції` |
| Cli | `ICommandStrategy` + 3 реалізації | категорії команд за ТЗ |
| Cli | `IRenderer<T>`, `IRenderable` | розділення виводу від логіки |
| Cli | `CharacterTableRenderer`, `CharacterCardRenderer` | два вигляди каталогу |
| Cli | `Repl` | цикл «рядок за рядком» поверх фасада |

## Автор

Марк Воскобойников, група КН-31 -
[github.com/Markentyy](https://github.com/Markentyy).

## Ліцензія

MIT - див. [LICENSE](LICENSE).

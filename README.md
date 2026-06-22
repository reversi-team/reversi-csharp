# Reversi

### C# implementation for Reversi (Othello)

---

# Для контриб'юторів

## Початок роботи

1. Клонуйте репозиторій:

```bash
git clone https://github.com/reversi-team/reversi-csharp.git
cd reversi-csharp
```

2. Створіть свою робочу гілку:

```bash
git checkout -b your-branch-name
```

Або в сучасніший спосіб:
```bash
git switch -c your-branch-name
```

3. Не забувайте підтягувати актуальні зміни з main. Завжди робіть це перед пушем:

```bash
git pull --rebase origin main
```

4. На кожну задачу створюйте окремий, інформативний коміт:

```bash
dotnet format        # виконайте щоб гарантувати відповідність форматування стандартам проєкту
git status           # перегляньте які файли змінились
git diff             # перегляньте що саме змінилося
git add file1 file2  # додайте необхідні файли до коміту
git commit -m "feat(model): implement moves validation"
```

5. Запуште зміни в гілку з відповідним ім'ям і відкрийте Pull Request:

```bash
git push -u origin your-branch-name
```

---

## Стандарти найменування

### 1. Назви гілок (Branch Naming)

Усі робочі гілки створюються від актуального стану main і повинні мати чіткий префікс, що вказує на тип задачі. Назви
пишуться нижнім регістром (lowercase) з використанням дефісів замість пробілів.

* **`feat/`** — розробка нового функціоналу.
    * *Приклад:* `feat/model-rules`, `feat/view-console-render`, `feat/model-bot-player`
* **`fix/`** — виправлення помилок у коді.
    * *Приклад:* `fix/model-disk-flip-logic`, `fix/view-score-counter-overflow`
* **`refactor/`** — зміна структури коду без зміни функціоналу або виправлення помилок.
    * *Приклад:* `refactor/clean-core-interfaces`, `refactor/optimize-model-board-array`

### Довготривалі гілки модулів

Якщо ви автономно розробляєте окремий великий модуль, допускається створення однієї базової гілки для цього компонента.
Використовуйте формат dev/<назва-модуля> (наприклад, `dev/model`, `dev/view` etc).

При роботі в такому форматі:

1. Робота розбивається на підзадачі. Проміжний готовий код регулярно злиється в main серією окремих Pull Request'ів з
   цієї гілки.
2. Гілка модуля не видаляється після першого мерджу і залишається активною до повного завершення роботи над компонентом.
3. Перед кожним створенням Pull Request автор обов'язково синхронізує свою гілку модуля з main через
   `git pull --rebase origin main`.

---

### 2. Повідомлення комітів (Commit Messages)

Коміти оформлюються суворо англійською мовою за спрощеним стандартом Conventional Commits. Структура повідомлення має
вигляд: `тип(область): опис дії в наказовій формі теперішнього часу`.

**Основні типи комітів:**

* **`feat:`** — додавання нової фічі.
    * *Приклад:* `feat(model): implement othello board initialization`
    * *Приклад:* `feat(view): add console grid rendering with ascii characters`
* **`fix:`** — виправлення багу.
    * *Приклад:* `fix(model): fix diagonal disk flipping direction`
    * *Приклад:* `fix(view): fix validation of incorrect moves`
* **`refactor:`** — рефакторинг коду.
    * *Приклад:* `refactor(model): optimize algorithm in GetPossibleMoves`
* **`chore:`** — зміни що не стосуються коду (документація, конфіги, ліцензії тощо).
    * *Приклад:* `chore(readme): improve development guidelines in README`

**Правила написання опису:**

1. Текст після двокрапки пишеться з маленької літери.
2. Опис має бути коротким, вказувати на те, що саме робить цей коміт (наприклад, `add...`, `fix...`, `update...`), без
   крапки в кінці.

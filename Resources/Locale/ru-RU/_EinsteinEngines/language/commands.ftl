command-list-langs-desc = Список языков вашей текущей сущности.
command-list-langs-help = Использование: {$command}

command-saylang-desc = Отправляет сообщение на указанном языке. Вы можете использовать как название языка, так и его позицию в списке.
command-saylang-help = Использование: {$command} <language id> <message>. Пример: {$command} TauCetiBasic "Hello World!". Пример: {$command} 1 "Hello World!"

command-language-select-desc = Выбирает текущий язык для сущности. Вы можете использовать как название языка, так и его позицию в списке.
command-language-select-help = Использование: {$command} <language id>. Пример: {$command} 1. Пример: {$command} TauCetiBasic

command-language-spoken = Говорит:
command-language-understood = Понимает:
command-language-current-entry = {$id}. {$language} - {$name} (current)
command-language-entry = {$id}. {$language} - {$name}

command-language-invalid-number = Номер языка должен быть между 0 и {$total}. В качестве альтернативы - используйте имя языка.
command-language-invalid-language = Язык {$id} не существует, или Вы не можете говорить на нем.

# Toolshed

command-description-language-add = Adds a new language to the piped entity. The two last arguments indicate whether it should be spoken/understood. Пример: 'self language:add "Canilunzt" true true'
command-description-language-rm = Removes a language from the piped entity. Works similarly to language:add. Пример: 'self language:rm "TauCetiBasic" true true'.
command-description-language-lsspoken = Lists all languages the entity can speak. Пример: 'self language:lsspoken'
command-description-language-lsunderstood = Lists all languages the entity can understand. Пример: 'self language:lsunderstood'

command-description-translator-addlang = Adds a new target language to the piped translator entity. See language:add for details.
command-description-translator-rmlang = Removes a target language from the piped translator entity. See language:rm for details.
command-description-translator-addrequired = Adds a new required language to the piped translator entity. Пример: 'ent 1234 translator:addrequired "TauCetiBasic"'
command-description-translator-rmrequired = Removes a required language from the piped translator entity. Пример: 'ent 1234 translator:rmrequired "TauCetiBasic"'
command-description-translator-lsspoken = Lists all spoken languages for the piped translator entity. Пример: 'ent 1234 translator:lsspoken'
command-description-translator-lsunderstood = Lists all understood languages for the piped translator entity. Пример: 'ent 1234 translator:lsunderstood'
command-description-translator-lsrequired = Lists all required languages for the piped translator entity. Пример: 'ent 1234 translator:lsrequired'

command-language-error-this-will-not-work = Это не сработает.
command-language-error-not-a-translator = Сущность {$entity} не является переводчиком.

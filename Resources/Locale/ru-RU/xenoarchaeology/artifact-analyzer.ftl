analysis-console-menu-title = Аналитическая консоль
analysis-console-server-list-button = Сервер
analysis-console-extract-button = Извлечь очки

analysis-console-info-no-scanner = Платформа не обнаружена! Соедените ее с консолью мультитулом.
analysis-console-info-no-artifact = Нет артефакта! Поставьте на его платформу для начала работы.
analysis-console-info-ready = Готово к сканированию.

analysis-console-no-node = Выберите узел для просмотра
analysis-console-info-id = [font="Monospace" size=11]ID:[/font]
analysis-console-info-id-value = [font="Monospace" size=11][color=yellow]{$id}[/color][/font]
analysis-console-info-class = [font="Monospace" size=11]Класс:[/font]
analysis-console-info-class-value = [font="Monospace" size=11]{$class}[/font]
analysis-console-info-locked = [font="Monospace" size=11]Статус:[/font]
analysis-console-info-locked-value = [font="Monospace" size=11][color={ $state ->
    [0] red]Locked
    [1] lime]Unlocked
    *[2] plum]Active
}[/color][/font]
analysis-console-info-durability = [font="Monospace" size=11]Прочность:[/font]
analysis-console-info-durability-value = [font="Monospace" size=11][color={$color}]{$current}/{$max}[/color][/font]
analysis-console-info-effect = [font="Monospace" size=11]Эффект:[/font]
analysis-console-info-effect-value = [font="Monospace" size=11][color=gray]{ $state ->
    [true] {$info}
    *[false] Unlock nodes to gain info
}[/color][/font]
analysis-console-info-trigger = [font="Monospace" size=11]Тригерры:[/font]
analysis-console-info-triggered-value = [font="Monospace" size=11][color=gray]{$triggers}[/color][/font]
analysis-console-info-scanner = Сканирование...
analysis-console-info-scanner-paused = На паузе.
analysis-console-progress-text = {$seconds ->
    [one] T-{$seconds} second
    *[other] T-{$seconds} seconds
}

analysis-console-extract-value = [font="Monospace" size=11][color=orange]Узел {$id} (+{$value})[/color][/font]
analysis-console-extract-none = [font="Monospace" size=11][color=orange] Ни один узел не имеет очков для извлечения[/color][/font]
analysis-console-extract-sum = [font="Monospace" size=11][color=orange]Всего Изучено: {$value}[/color][/font]

analyzer-artifact-extract-popup = Энергия мерцает на поверхности артефакта!
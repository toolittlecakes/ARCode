# ARCode

На данный момент проект состоит из четырех модулей:

1. Само приложение, которое трекает AR-коды, загружает сцены, и отрисовывает их.
2. Редактор сцен. Представляет из себя обычный проект Unity с префабом ARCodePlaceholder, положение которого задает положение относительно затреканного AR-кода, скрипт сборки AssetBundle'а и пример сцены.
3. Сервер, который получает запросы по адресу, указанному на qr-коде и возвращает адрес, по которому расположена сцена
4. Сервер - база данных со всеми сценами. Понятно, что они могут храниться где угодно.

Первый два лежат тут, последние два на [repl.it](http://repl.it) (гуглдок для кода), потому что так редачить проще и запуск на их серверах, где они крутяться 24 часа в сутки.

Вот ссылки на их редактирование:

ARCode: [https://repl.it/join/fgtdvgql-nikolaishieiko](https://repl.it/join/fgtdvgql-nikolaishieiko)

ARStorage: [https://repl.it/join/uquyizaf-nikolaishieiko](https://repl.it/join/uquyizaf-nikolaishieiko)

### ARLens:

Скачивается SDK отсюда, добавляется в проект через assets -> import package -> custom package.

Возможно ARcamera в сцене битая, нормальную можно достать в MaxstAR->Prefabs. В параметрах устанавливается центр мира - треккер, в конфиге лицензионный ключ:
Fkl2Dag0Qliaeuc3J5f2zqQETl90ATGJOf6zJUCzV8c=

Вся суть в папке LocalAssets, все остальное это SDK Maxst(upd: я ее уже выпилил из репы). Там есть примеры кода и сцен, но они как-то по-дурацки сделаны, я в них просто методы взаимодействия с API смотрел.

ARTracker - объект, отвечающий за треккинг AR-кодов, скачивание сцен, компоновку скриптов с объектами сцены, отсчет сцены относительно ARCodePlaceholder'а и отрисовку всего этого дерьма. Декомпозиция и принцип единственности ответственности, епта. 

В ARCamera можно задавать что будет перемещаться на сцене - камера или трекер. Отличия в положении источника света. Если перемещается камера, то на созданные объекты свет будет светить независимо от того подносим камеру к объекту или нет. В конфигурации камеры нужно вводить лицензионный ключ, полученный на сайте Maxst. Вроде должен прокатить тот же самый.

Для того чтобы тестить на локальной машине не держа постоянно телефон с открытым кодом перед вебкамерой, там есть сцена Debug, юзающая скрипт DownloadAndRun. Там нужно захардкодить путь до файла, который хочешь загрузить, после чего он загрузится и приаттачит скрипт.

### ARScene:

1. Поместить EmptyObject на сцену Unity. В дальшейшем буду называть его Scene
2. Поместить все желаемые объекты в него. Добавить скрипты. 
3. Поместить ARCodePlaceholder дочерним объектом Scene.
4. Настроить его позицию и поворот - оно будет соответствовать позиции реального AR кода относительно сцены
5. Сделать префаб Scene: перетащить получившуюся иерархию объектов в папку Prefabs.
6. Добавить Assembly definition в папку со скриптами. Назовем его Scripts
7. Из папки {ProjectName}/Library/ScriptAssemblies скопировать Scripts.dll в папку Prefabs, поменяв имя на Scripts.bytes.
8. К этому шагу в папке Prefabs должны находиться два файла: Scene.prefab и Scripts.bytes. Для каждого из них задать один и тот же ассет в окне inspector внизу.
9. В меню  Unity тыкнуть Assets→BuildAssetBundle 
10. В папке AssetBundle появяться несколько файлов. Тот который без расширений - нужный ассетбандл. Его можно помещать на сервер.

Там в папке ARTools/Example лежит пример, как это сделано.

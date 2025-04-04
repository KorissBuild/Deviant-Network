## Перед использованием нужно установить Unity, качать здесь: https://unity.com/
для правильной работы необходимо установить Unity 6-ой версии (Unity 6000.0.43f1).

# Deviant Network - простой и опенсурсный мессенджер, предназначенный для полностью анонимного общения. 
Мессенджер НЕ хранит переписки или медию на базах данных.
Никто не имеет доступа к информации в беседах, кроме самих участников. Передача данных осуществляется с помощью RPC-системы и отображается локально на всех клиентах через UI-инструменты Unity.

### Инструменты для работы
#### Приложение создано на Unity 6, сетевая часть это Photon Engine, в качестве базы данных выступает Firebase.

### Добавление своей базы данных
* После открытия архива, перейдите на https://firebase.google.com и создайте базу данных для Unity. Придумайте и введите ID вашего проекта. 
* В меню Project Overview выберите Unity, после этого в "Register as Android app" введите Project ID вашего приложения из Unity Player Settings.
* Скачайте файл google-services.json и перекиньте его в папку Assets вашего проекта.
* Скачайте Firebase Unity SDK и импортируйте в Unity следующие пакеты: FirebaseAuth, FirebaseDatabase.
* Далее в разделе Build подключите Authentication (выберите Email/Password) и Realtime Database (начните в test mode).

После шагов выше, ошибки в консоли должны пропасть.

### Добавление сетевой технологии
* Перейдите на https://dashboard.photonengine.com, нажмите Create a new app и создайте аппликацию типа PUN 2.
* Скопируйте PUN ID, в Unity нажмите Windows > Photon Unity Networking > PUN Wizard > Locate PhotonServerSettings, и в App Id PUN введите ваш ID.

## Все готово! Теперь у вас есть рабочая версия мессенджера, с которой вы можете делать что угодно.

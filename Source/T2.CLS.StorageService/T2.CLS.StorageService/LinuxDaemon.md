# Развёртывание службы под Linux

## Предварительные требования

На развёртываемой машине должен быть установлен:
- .Net Core рантайм версии 3.1
- Clickhouse сервер
- ODBC драйвер Clickhouse
- unixodbc

## Сборка проекта

Необходимо выполнить сборку проекта.

## Настройка службы

#### Подготовка
Сначала необходимо создать пользователя, от имени которого будет работать служба

	adduser t2-cls

Создать директорию и скопировать в неё службу:

	mkdir /t2/t2-cls-storaged -p

Назначить пользователя владельцем данной директории:

	cd /t2
	chown -R t2-cls:t2-cls t2-cls-storaged

Т.к. служба занимается архивированием устаревших партиций, у неё должен быть доступ к директории данных Clickhouse, для этого необходимо добавить созданного пользователя в состав группы clickhouse

    usermod -aG clickhouse t2-cls

#### Настройка ODBC источника подключения к БД

В ```/etc/odbc.ini``` добавить настройку подключения к БД:

    [ODBC Data Sources]
    CH = CH
    
    [CH]
    Driver=/usr/src/clickhouse-odbc/build/driver/libclickhouseodbcw.so
    Server=127.0.0.1
    Database=default
    Port=8123
    Username=default

где ```/usr/src/clickhouse-odbc/build/driver/libclickhouseodbcw.so``` путь к скомпилированному ODBC драйверу Clickhouse.

#### Конфигурация службы
В настройках службы (файл ```appsettings.json```) указать подключение к БД а также путь к директории данных Clickhouse и путь хранению архивов:

    {
      "Storage": {
        "ConnectionString": "DSN=CH",
        "DataPath": "/var/lib/clickhouse/data",
        "ArchivePath": "/var/lib/clickhouse/archive"
      }
    }

Проверить, что служба запускается и работает (при условии, что dotnet находится в директории /usr/share/dotnet):

    /usr/share/dotnet/dotnet /t2/t2-cls-storaged/T2.CLS.StorageService.dll


#### Создание демона

В директории ```/usr/lib/systemd/system``` создать конфигурацию службы ```t2-cls-storaged.service``` с содержимым:

    [Unit]  
    Description=T2 CLS Storage Service  
      
    [Service]  
    ExecStart=/usr/share/dotnet/dotnet /t2/t2-cls-storaged/T2.CLS.StorageService.dll
    WorkingDirectory=/t2/t2-cls-storaged
    User=t2-cls  
    Group=t2-cls
    Restart=on-failure  
    SyslogIdentifier=t2-cls-storaged 
    PrivateTmp=true  
      
    [Install]  
    WantedBy=multi-user.target

Перезагрузить конфигурацию служб и создать метассылку для нашей службы:

    systemctl daemon-reload  
    systemctl enable t2-cls-storaged

Запустить службу и проверить её статус:

    systemctl start t2-cls-storaged
    systemctl status t2-cls-storaged


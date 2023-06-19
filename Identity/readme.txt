Перед использованием
1. Сгенерировать новые ключи для подписывания токенов, 
   Для этого идем в DevelopmentTests.GenRsa(), содержимое вывода вставить в sign-token-key.json
2. Сгенерировать новые секреты, для Identity  
   Для этого идем в DevelopmentTests.GenSecrets(), содержимое вывода вставить в SecretConfig
   После изменения SecretConfig изменить конфиги сервисов
        Например Admin.appsettings.json > "ApiResourceSecret": "AA6F7FEC-8793-4B03-8BAA-BF8EDCF9F787", взять значение из нового конфига.
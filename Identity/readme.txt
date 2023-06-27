Перед использованием
1. Сгенерировать новые ключи для подписывания токенов, 
   Для этого идем в DevelopmentTests.GenRsa(), содержимое вывода вставить в sign-token-key.json
2. Сгенерировать новые секреты, для Identity  
   Для этого идем в DevelopmentTests.GenSecrets(), содержимое вывода вставить в SecretConfig
   После изменения SecretConfig изменить конфиги сервисов
        Например Service.appsettings.json > "ApiResourceSecret": "E88D8933-E5F3-4FF0-AE28-A33DA67244E5", взять значение из нового конфига.
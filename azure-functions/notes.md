poznámky pro příště
 * Do prace nahradni triko, hlavne kdyz bude talk nebo tak
 * Meetup - priste ne tak kontrastni pozadi (může být i ve tmě, takže VS code předem na solarized dark, obrázky v přednášce ne bílé, ale aspoň trochu šedé pozadí)



- neni talk primárně o serverless, odkazat na prednasku Gojko. (nebudu řešit obecné principy, výhody a tak, když tak jen krátce)
- serverless jen lehce zminit, spis motivaci k pouziti funkci
- jak to funguje, jake verze
- vstupy, vystupy, napojeni
- ukazky (smart - Cs i Fs)
- fsharping - ukazky hlavne v1, kvuli toolingu. Ale i navod, jak pouzit v2 (musim do te doby rozchodit - kdyz cas a moznost tak pripravit i v2 template? Je to opensource)
- HTTP funkce asi schovat za API gateway? něco k tomu najít
- log level settings v host.json https://stackoverflow.com/questions/53405020/configure-loglevel-of-azure-function-using-environment-variables
- ukázat ExecutionContext (čtení konfigurace)
- reálný příklad - jak strukturovat. viz SwitchMonitoring
- vývoj: "AzureWebJobsStorage": "UseDevelopmentStorage=true", + jak nastavit po nasazení
    musí běžet Azure Storage Emulator
    https://stackoverflow.com/questions/33765141/azure-table-storage-no-connection-could-be-made-because-the-target-machine-act
    https://markgossa.blogspot.com/2018/12/azure-function-listener-for-function.html
    https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator !
    Emulator init problems
      - https://stackoverflow.com/questions/53673763/azure-storage-emulator-fails-to-init-with-the-database-azurestorageemulatordb5
      - create DB manually (clashes with something, SQL 2019? or LocalDB? who knows)
    Emulator run problems
       - The listener for function 'RemoveDevLogsTimer' was unable to start. Microsoft.WindowsAzure.Storage: Server encountered an internal error. Please try again after some time.
       - asi práva?
        - 5.8 a vyšší používá jako default `blobs` a ty nefungují? to je asi přístupem k té DB řekl bych..
        viz https://blog.maximerouiller.com/post/fixing-azure-functions-and-azure-storage-emulator-58-issue/ a https://github.com/Azure/azure-functions-host/wiki/Changes-to-Key-Management-in-Functions-V2
        ale tohle taky nepomůže
      - > AzureStorageEmulator.exe init -forceCreate -inprocess               

    poslední zkoušený postup
     * ručně vytvořit DB v cestě c:\Work\localDBs\ (mimo user account)
     * AzureStorageEmulator.exe  init, s parametrem -inprocess
     * emulator start
     * => stále stejná chyba, vzdávám to a připojím se k Azure.
     * našel jsem logy - problém je, že se nevytvoří tabulky. hm, hm, hm. a init nefunguje
     


SMART Azure Functions - určitě zmínit security, že lokálně projde vše, ale je kontrolováno na Azure. ukázat jak nastavit, co které nastavení znamená


Durable Functions
https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-bindings


* https://blog.jetbrains.com/dotnet/2019/05/09/building-azure-functions-sql-database-improvements-azure-toolkit-rider-2019-1/

*   https://azure.microsoft.com/cs-cz/free/ Functions free? imo nepůjde bez non-free plánu
            ha functions navždy zdarma. beru :)
* https://azure.microsoft.com/en-us/features/storage-explorer/ pro kontrolu storage



https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices

https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/


projít best practices, antipatterns - s tím HttpClient jsem to ale nepochopil https://github.com/Azure/azure-functions-host/issues/1806
aha, je to tady https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections
  zdá se, že stačí použít static - otestovat, ale bylo by to fajn



https://serverlesslibrary.net/ - Azure Functions and Logic Apps - open source use cases



Azure Storage Emulator - https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator

Queue settings - https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-queue#hostjson-settings
TODO nastudovat zpracování, poison queue v tomto případě

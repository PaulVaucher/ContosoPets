namespace ContosoPets.Domain.Constants
{
    /// <summary>
    /// Logging constants
    /// </summary>
    public abstract class LoggingConstants
    {
        // Application lifecycle events
        public const string ApplicationStarting = "Application starting";
        public const string ApplicationStarted = "Application started successfully";
        public const string ApplicationShuttingDown = "Application shutting down gracefully";
        public const string ConfigurationLoaded = "Configuration loaded from source: {ConfigurationSource}";

        // Service layer operations
        public const string ServiceOperationStarted = "Starting {Operation} for {EntityType}";
        public const string ServiceOperationCompleted = "Completed {Operation} for {EntityType} in {ElapsedMs}ms";
        public const string ServiceOperationFailed = "Failed {Operation} for {EntityType}: {ErrorReason}";

        // Domain operations
        public const string AnimalCreationStarted = "Creating new animal: Species={Species}";
        public const string AnimalCreationCompleted = "Successfully created animal: Id={AnimalId}, Species={Species}";
        public const string AnimalValidationFailed = "Animal validation failed: {ValidationError}";
        public const string AnimalNotFound = "Animal not found with Id: {AnimalId}";

        // Repository operations
        public const string RepositoryOperationStarted = "Starting repository operation: {Operation}";
        public const string RepositoryOperationCompleted = "Repository operation completed: {Operation}, RecordsAffected={RecordsCount}";
        public const string DatabaseTransactionStarted = "Database transaction started for operation: {Operation}";
        public const string DatabaseTransactionCommitted = "Database transaction committed successfully";
        public const string DatabaseTransactionRolledBack = "Database transaction rolled back due to error";
        public const string DatabaseInitializationStarted = "Database initialization started";
        public const string DatabaseInitializationCompleted = "Database initialization completed successfully";

        // Search and filtering operations
        public const string SearchOperationStarted = "Starting search: Species={Species}, Characteristic={Characteristic}";
        public const string SearchOperationCompleted = "Search completed: Found {ResultCount} animals matching criteria";
        public const string FilteringIncompleteData = "Filtering animals with incomplete {DataType}";

        // User interaction events
        public const string UserMenuSelection = "User selected menu option: {MenuOption}";
        public const string UserInputReceived = "User input received for {InputType}: {InputValue}";
        public const string InvalidUserInput = "Invalid user input received for {InputType}: {InputValue}";

        // Performance and monitoring
        public const string PerformanceMetric = "Performance metric - {Operation}: {Duration}ms";
        public const string MemoryUsage = "Memory usage after {Operation}: {MemoryBytes} bytes";

        // Error context enhancement
        public const string ErrorContext = "Error context - Operation={Operation}, EntityId={EntityId}, UserId={UserId}";
        public const string UnexpectedError = "Unexpected error occurred in {ClassName}.{MethodName}";

        // Debug information
        public const string ObjectStateChange = "Object state changed: {ObjectType} {ObjectId} - {PropertyName}: '{OldValue}' -> '{NewValue}'";
        public const string ValidationRuleApplied = "Validation rule applied: {RuleName} on {EntityType} {EntityId}";
        public const string BusinessRuleEvaluated = "Business rule evaluated: {RuleName}, Result={Result}";

        // Method-specific logging
        public const string RetrievingAnimalById = "Retrieving animal by ID: {AnimalId}";
        public const string AnimalFoundWithDetails = "Found animal: Species={Species}, Nickname={Nickname}";
        public const string CurrentPetCount = "Current pet count: {PetCount}";
        public const string SearchWithEmptyParameters = "Search called with empty parameters: Species='{Species}', Characteristic='{Characteristic}'";
        public const string UpdateAnimalsNullCorrections = "UpdateAnimalsFromCorrections called with null request";
        public const string UpdatingAnimalsWithCorrections = "Updating {AnimalCount} animals with corrections";
        public const string AnimalUpdatedSuccessfully = "Successfully updated animal {AnimalId}";
        public const string AnimalUpdateFailed = "Failed to update animal {AnimalId}: {ErrorMessage}";
        public const string NoValidModificationsForAnimal = "No valid modifications provided for animal {AnimalId}";
        public const string BatchUpdateStarted = "Starting batch update for {UpdateType}";
        public const string BatchUpdateCompleted = "Completed batch update for {UpdateType}";

        // Command execution logging
        public const string ExecutingCommand = "Executing command: {CommandType}";
        public const string CommandExecutedSuccessfully = "Command executed successfully: {CommandType}";
        public const string CommandExecutionFailed = "Command execution failed: {CommandType}";

        // Validation and business rules
        public const string ValidationNullRequest = "AddNewAnimal called with null request";
        public const string ValidationEmptyCorrections = "UpdateAnimalsFromCorrections called with no corrections";

        // Scope context values
        public const string OperationListAllAnimals = "ListAllAnimals";
        public const string OperationAddNewAnimal = "AddNewAnimal";
        public const string OperationGetIncompleteAgeOrDescription = "GetIncompleteAgeOrDescription";
        public const string OperationGetIncompleteNicknameOrPersonality = "GetIncompleteNicknameOrPersonality";
        public const string OperationUpdateAge = "UpdateAge";
        public const string OperationUpdatePersonality = "UpdatePersonality";
        public const string OperationSearchByCharacteristic = "SearchByCharacteristic";
        public const string OperationCompleteAgesAndDescriptions = "CompleteAgesAndDescriptions";
        public const string OperationCompleteNicknamesAndPersonality = "CompleteNicknamesAndPersonality";

        // Entity and operation types
        public const string EntityTypeAnimal = "Animal";
        public const string OperationTypeListAll = "ListAll";
        public const string DataTypeAgeOrDescription = "Age or Description";
        public const string DataTypeNicknameOrPersonality = "Nickname or Personality";
        public const string UpdateTypeAgesAndDescriptions = "ages and descriptions";
        public const string UpdateTypeNicknamesAndPersonality = "nicknames and personality";
        public const string PropertyAge = "Age";
        public const string PropertyPersonalityDescription = "PersonalityDescription";
        public const string PropertyPhysicalDescription = "PhysicalDescription";
        public const string PropertyNickname = "Nickname";

        // Input types for logging
        public const string InputTypeMenuSelection = "MenuSelection";
        public const string InputValueNull = "null";
    }
}
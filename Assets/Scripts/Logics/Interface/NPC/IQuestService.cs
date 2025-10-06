public interface IQuestService
{
    string MapKey { get; }
    void ActivateQuest(string key);
}

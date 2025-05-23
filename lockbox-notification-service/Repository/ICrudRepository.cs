namespace lockbox_notification_service.Repository;

public interface ICrudRepository<TModel>
{
    void Create(TModel model);
    TModel? Read(string id);
    void Update(TModel model);
    void Delete(string id);
}
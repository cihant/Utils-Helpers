public class SessionHelper<T> 
{
    private string Key;

    public SessionHelper()
    {
        this.Key = typeof(T).Name; 
    }

    public bool IsNotNull()
    {
        return this.Get() != null;
    }

    public bool IsNull()
    {
        return !this.IsNotNull();
    }

    public void Save(object value)
    {
        HttpContext.Current.Session[Key] = value;
    }

    public T Get()
    {
        return (T)HttpContext.Current.Session[Key];
    }

    public void Destroy()
    {
        HttpContext.Current.Session.Remove(Key);
    }

    public static void Destroy(string key)
    {
        HttpContext.Current.Session.Remove(key);
    }
}
/*
///how to use 
public ActionResult Test() {
  SessionHelper<PostRequestModel> sh = new SessionHelper<PostRequestModel>();   
  if (sh.IsNull()) return HttpNotFound();
  
  return View();
}
*/

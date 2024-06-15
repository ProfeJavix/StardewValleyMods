namespace OSTPlayer
{
    public interface IPropChangeListener<T>
    {
        void OnPropChanged(T newValue);
    }
}

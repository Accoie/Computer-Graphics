namespace Task2.Strategies;

public interface IRenderStrategy<T>
{
    void Render(T entity);
}

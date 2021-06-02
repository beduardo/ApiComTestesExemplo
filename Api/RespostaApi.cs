namespace Api
{
    public class RespostaApi<T>
    {
        public T dados { get; set; }
        public bool erro { get; set; }
        public string mensagem { get; set; }
    }
}
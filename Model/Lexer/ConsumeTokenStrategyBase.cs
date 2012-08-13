namespace Model.Lexer
{
    using Model.Lexer.Tokens;

    public class ConsumeTokenStrategyBase
    {
        protected ConsumeTokenStrategyBase(IIgnoreTokenStrategy ignoreTokenStrategy)
        {
            this.IgnoreTokenStrategy = ignoreTokenStrategy;
        }

        protected internal IIgnoreTokenStrategy IgnoreTokenStrategy { get; private set; }

        public virtual bool IsTokenToBeIgnored(TokenBase token)
        {
            return this.IgnoreTokenStrategy.IsTokenToBeIgnored(token);
        }
    }
}
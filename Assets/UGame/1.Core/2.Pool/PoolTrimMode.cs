namespace UGame
{
    /// <summary>
    /// 对象池在达到最大存储上限时，对已存在的对象使用的销毁模式
    /// </summary>
    public enum PoolTrimMode
    {
        //立即销毁
        Immediate,
        //延时销毁
        Delay
    }
}
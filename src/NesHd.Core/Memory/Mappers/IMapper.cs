namespace NesHd.Core.Memory.Mappers
{
    public interface IMapper
    {
        /// <summary>
        /// Get if this mapper accepts values written from addresses under 0x8000
        /// </summary>
        bool WriteUnder8000 { get; }

        /// <summary>
        /// Get if this mapper accepts values written from addresses under 0x6000
        /// </summary>
        bool WriteUnder6000 { get; }

        /// <summary>
        /// Write a value into this mapper
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="data">The value</param>
        void Write(ushort address, byte data);

        /// <summary>
        /// Setup the mapper (reset)
        /// </summary>
        void SetUpMapperDefaults();

        /// <summary>
        /// Tick the timer that needs to tick in each scanline excapt the VBlank
        /// </summary>
        void TickScanlineTimer();

        /// <summary>
        /// Tick the timer that needs to tick in each CPU cycle
        /// </summary>
        void TickCycleTimer();

        /// <summary>
        /// Soft Reset the mapper
        /// </summary>
        void SoftReset();
    }
}
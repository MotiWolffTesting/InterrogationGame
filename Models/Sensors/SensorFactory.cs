namespace InterrogationGame.Models.Sensors;

public class SensorFactory
{
    public ISensor CreateSensor(SensorType type)
    {
        return type switch
        {
            SensorType.Signal => new SignalSensor(),
            SensorType.Magnetic => new MagneticSensor(),
            SensorType.Light => new LightSensor(),
            SensorType.Thermal => new ThermalSensor(),
            SensorType.Motion => new MotionSensor(),
            SensorType.Chemical => new ChemicalSensor(),
            SensorType.Audio => new AudioSensor(),
            SensorType.Pulse => new PulseSensor(),
            _ => throw new ArgumentException($"Unknown sensor type: {type}")
        };
    }
} 
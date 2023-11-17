namespace AiAssaultArena.Contract;
public class ParametersResponse
{
    //TANK
    public float MomentOfInertia => 1f / 12f * Mass * (Width * Width + Height * Height);
    public float Width { get; set; }= 40;
    public float Height { get; set; }= 60;
    public float Density { get; set; }= 78; // kg/dm³
    public float Mass => Density * Width * Height * 40;
    public float TurretLength { get; set; }= 50;
    public float MaxAcceleration { get; set; }= 50f;
    public float MaxSpeed { get; set; }= 100f;
    public float TurretRotationSpeed { get; set; }= 0.5f;
    public float SensorRotationSpeed { get; set; }= 0.7f;
    public float Friction { get; set; }= 0.99f;
    public float AngularFriction { get; set; }= 0.5f;
    public float LateralDamping { get; set; }= 0.25f;

    //BULLET
    public float Speed { get; set; } = 150f;
    public float Radius { get; set; }  = 5f;
}

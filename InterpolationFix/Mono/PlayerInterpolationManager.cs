using UnityEngine;

namespace InterpolationFix.Mono;

public class PlayerInterpolationManager : MonoBehaviour
{
    private GroundMotor groundMotor;
    
    //state
    private bool registeredForHighFixedTimestep;
    private Rigidbody registeredRigidbody;
    private WorldForces registeredWorldForces;

    public void Start()
    {
        groundMotor = gameObject.GetComponent<GroundMotor>();
    }
    
    public void Update()
    {
        Transform platform = groundMotor.movingPlatform.hitPlatform;// either null or collider player is standing on

        if (!platform || Player.main.mode == Player.Mode.Piloting)//If the player camera is locked (like when piloting), no need for fixes
        {
            if (registeredForHighFixedTimestep)
            {
                EnableInterpolation();//cleanup last active platform
            }
            
            return;//we are on nothing
        }
        
        Rigidbody platformRb = platform.GetComponentInParent<Rigidbody>();//expensive but the only way to be sure

        if (!platformRb)
        {
            if (registeredForHighFixedTimestep)
            {
                EnableInterpolation();//cleanup last active platform
            }
            
            return;//we arnt on a rigidbody, just some other object. No interpolation here
        }
        
        registeredRigidbody = platformRb;
        registeredWorldForces = platform.GetComponentInParent<WorldForces>();

        bool highPrecisionRequired = RigidbodyRequiresHighPrecisionPhysics(platformRb);

        if (highPrecisionRequired && !registeredForHighFixedTimestep)
        {
            DisableInterpolation();
        }
        if (!highPrecisionRequired && registeredForHighFixedTimestep)
        {
            EnableInterpolation();
        }
    }
    
    public bool RigidbodyRequiresHighPrecisionPhysics(Rigidbody platform)
    {
        return platform.velocity.sqrMagnitude > 0.001f || platform.angularVelocity.sqrMagnitude > 0.001f;
    }

    private void DisableInterpolation()
    {
        MainGameController.instance.RegisterHighFixedTimestepBehavior(this);
        
        if(registeredRigidbody) registeredRigidbody.interpolation = RigidbodyInterpolation.None;
        if(registeredWorldForces) registeredWorldForces.lockInterpolation = true;
        
        registeredForHighFixedTimestep = true;
    }
    
    private void EnableInterpolation()
    {
        MainGameController.instance.DeregisterHighFixedTimestepBehavior(this);
        
        if(registeredRigidbody) registeredRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        if(registeredWorldForces) registeredWorldForces.lockInterpolation = false;
        
        registeredForHighFixedTimestep = false;
    }
    
}
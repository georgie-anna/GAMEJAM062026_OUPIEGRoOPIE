using UnityEngine;

public class LookBehaviour
{

    //--- Dependencies ---

    private readonly LookConfig _lookConfig;
    private readonly Rigidbody _rb;
    private readonly Transform _camTransform;

    //--- Fields ----
    private float _yaw;
    private float _pitch;



    public LookBehaviour(Rigidbody rb,LookConfig lookConfig,Transform camTransform)
    {
        _lookConfig=lookConfig;
        _rb=rb;
        _camTransform=camTransform;
    }

    public void Look(Vector2 lookInput)
    {
        _yaw+=lookInput.x* _lookConfig.Sensitivity;
        _pitch-=lookInput.y* _lookConfig.Sensitivity;
        _pitch = Mathf.Clamp(_pitch, _lookConfig.MinLookDown, _lookConfig.MaxLookUp);


        _rb.rotation= Quaternion.Euler(0, _yaw, 0);
        _camTransform.localRotation=Quaternion.Euler(_pitch, 0, 0);





    }




}

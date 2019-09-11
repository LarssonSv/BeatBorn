using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCanvasElement : MonoBehaviour
{
    RectTransform _transform;
    private Camera _mainCamera;
    public float SizeInUnits = 2;
    /*
    private float _verticalFOVInRads { get { return _mainCamera.fieldOfView * Mathf.Deg2Rad; } }

    private float _help_HFOVInRads = 0f;
    private float _help_cameraAspect = 0f;
    private float _horizontalFOVInRads
    {
        get
        {
            if(_mainCamera.aspect != _help_cameraAspect)
            {
                _help_cameraAspect = _mainCamera.aspect;
                _help_HFOVInRads = 2 * Mathf.Atan(Mathf.Tan(_verticalFOVInRads / 2) * _mainCamera.aspect);
            }
            return _help_HFOVInRads;
        }
    }
    private float _horizontalFOV => _horizontalFOVInRads * Mathf.Rad2Deg;
    */
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<RectTransform>();
        _mainCamera = FindObjectOfType<Camera>();
        
        Vector3 screenSize = new Vector3(  _transform.localScale.x / _transform.rect.width,
                                         _transform.localScale.y/ _transform.rect.height,
                                         _transform.localScale.y / _transform.rect.height ) * SizeInUnits;

        
        Vector2 screenPercentage = new Vector2(screenSize.x / Screen.width, (screenSize.y * _mainCamera.aspect)/ Screen.width) * Screen.width / 1920f;


        Vector2 screenPos = _transform.anchoredPosition;
        
   
        
        Vector3 topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));
        Vector3 topLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 1, _mainCamera.nearClipPlane));
        Vector3 bottomRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0, _mainCamera.nearClipPlane));
        Vector3 bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
        
        
        transform.SetParent(_mainCamera.transform);
        transform.localScale = new Vector3((bottomLeft - bottomRight).magnitude * screenPercentage.x,
            (topLeft - bottomLeft).magnitude * screenPercentage.y);
        transform.forward = _mainCamera.transform.forward;
        _transform.position = bottomLeft;
        _transform.localPosition += Vector3.forward * 0.0001f;

        float width = (bottomLeft - bottomRight).magnitude;
        float height = (topLeft - bottomLeft).magnitude;
        
        _transform.localPosition += width * Vector3.right * _transform.anchorMin.x;
        _transform.localPosition += width * Vector3.right * (screenPos.x / 1920);
        
        _transform.localPosition += height * Vector3.up * _transform.anchorMin.y;
        _transform.localPosition += height * Vector3.up * (screenPos.y / 1080);
  
    }

    private void Update()
    {
    //    Vector3 xEdgeDirection = Quaternion.Euler(0, _horizontalFOV * 0.5f, 0) * -_mainCamera.transform.forward;
     //   Vector3 yEdgeDirection = Quaternion.Euler(-_mainCamera.fieldOfView * 0.5f, 0 , 0) * -_mainCamera.transform.forward;


    // Debug.DrawLine(topLeft, topRight, Color.green);
        
    //    Debug.DrawLine(topRight,  botomLeft, Color.red);
    }
}

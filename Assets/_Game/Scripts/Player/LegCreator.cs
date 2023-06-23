using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Player
{
    public class LegCreator : MonoBehaviour
    {
        private int _pointsCount=20;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField,Range(0.001f,1f)] private float _renderValue;
        private Coroutine _moveLegCoroutine;
        private bool _updateLegPos=true;
        private Vector3[] _ptEvalBuffer;
        private List<Vector3> _points;
        private Vector3[] _setPoints;
        private Vector3 _pOffset;
        private float _rotationTime;
        private float _rotationSpeed = 2f;
        
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
        private void Awake()
        {
            _points = new List<Vector3>() {Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero};
            _ptEvalBuffer = new Vector3[_points.Count-1];
            _setPoints = new Vector3[_points.Count-1];

        }

        public void SetUpRenderValue(float value)
        {
            _renderValue = value;
        }

        public void SetBodyPosition(Vector3 pos)
        {
            _points[0] = pos;
        }

        public void LockLeg()
        {
            _updateLegPos = true;
        }

        public void StartMove()
        {
            _rotationTime=0f;
            _moveLegCoroutine = StartCoroutine(MoveLeg());
            _updateLegPos = false;
        }

        public void SetFootPosition(Vector3 position)
        {
            _points[_points.Count-1] = position;
        }

        public bool LegTooLong(Transform character)
        {
            if ((( _points[0]+character.forward*5f) - _points[_points.Count-1]).sqrMagnitude > 80f&&!_updateLegPos)
                return true;
            return false;
        }

        public void StopMove()
        {
            _updateLegPos = true;
            StopCoroutine(_moveLegCoroutine);
        }
        
        public void SetPoints()
        {
          
            for (int i = 1; i < _points.Count-1; i++)
            {
                _points[i] = Vector3.Lerp(_points[0], _points[_points.Count-1], (1f/_points.Count)*i);
                if (i <= _points.Count-2 / 2)
                {
                    _setPoints[i] = Vector3.up*Random.Range(0,2f)+Vector3.left*Random.Range(-1f,1f);
                }
                else
                {
                    _setPoints[i] = Vector3.down*Random.Range(0,0.01f)+Vector3.left*Random.Range(-0.01f,-0.01f);
                }
                
                _points[i] += _setPoints[i];

            }
        }
        
        private void DoCurveAnimation(float radius)
        {
            _rotationTime += _rotationSpeed * Time.deltaTime;
            float x;
            float z;
            float newRadius;
            for (int i = 1; i < _points.Count-1; i++)
            {
                newRadius = radius * 1 / _points.Count - 1 / i;
                if (i % 2 == 0)
                {
                    x = Mathf.Cos(_rotationTime) * newRadius;
                    z = Mathf.Sin(_rotationTime) * newRadius;
                }
                else
                {
                    x = Mathf.Cos(-_rotationTime) * newRadius;
                    z = Mathf.Sin(-_rotationTime) * newRadius;  
                }
                Vector3 position = new Vector3(x, z, 0);
                Vector3 pos = Vector3.Lerp(_points[0], _points[_points.Count-1], (1f/_points.Count)*i);
                pos += _setPoints[i]+position;
                Vector3 newPos = ((pos ) - _points[i]);
                _points[i] +=newPos*Time.deltaTime*_rotationSpeed*4 ;
                
                
            }
        }

        private int Count {
            [MethodImpl( INLINE )] get => _points.Count;
        }
        private Vector3 Eval( float t ) {
            float n = Count - 1;
            for( int i = 0; i < n; i++ )
                _ptEvalBuffer[i] = Vector3.LerpUnclamped( _points[i], _points[i + 1], t );
            while( n > 1 ) {
                n--;
                for( int i = 0; i < n; i++ )
                    _ptEvalBuffer[i] = Vector3.LerpUnclamped( _ptEvalBuffer[i], _ptEvalBuffer[i + 1], t );
            }

            return _ptEvalBuffer[0];
        }

        private IEnumerator MoveLeg()
        {
            float radius = Random.Range(0f, 1f);
            while (true)
            {
                _lineRenderer.positionCount = _pointsCount;
                Vector3[] array = new Vector3[_pointsCount];
                for (int i = 0; i < _pointsCount; i++)
                {
                    float t;
                    if (i == 0)
                        t = 0f;
                    else if (i == _pointsCount - 1&&_renderValue==1f)
                        t = 1f;
                    else
                        t = 1f / ((float) _pointsCount / i/_renderValue);

                    array[i] = Eval(t);
                }

               DoCurveAnimation(radius);
                _lineRenderer.SetPositions(array);
                yield return null;
            }
        }
        

        
    }
}

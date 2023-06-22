using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Freya;
using NaughtyAttributes;
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
        float randomNumber;
        float lastNumber;
        int maxAttempts = 10;
        private Vector3[] _ptEvalBuffer;
        private List<Vector3> _points;
        private Vector3[] _setPoints;

        private Vector3 _pOffset;
        
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
            _moveLegCoroutine = StartCoroutine(MoveLeg());
            _updateLegPos = false;
        }

        public void SetFootPosition(Vector3 position)
        {
            _points[_points.Count-1] = position;
        }

        public bool LegTooLong(Transform character)
        {
            if ((( _points[0]+character.forward*5f) - _points[_points.Count-1]).sqrMagnitude > 60f&&!_updateLegPos)
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
                _setPoints[i] = Vector3.up*Random.Range(-1f,1f)+Vector3.left*Random.Range(-1f,1f);
                _points[i] += _setPoints[i];
                _points[i]+=Vector3.up*Random.Range(-0.5f,0.5f);

            }
            

        }

        private void OnDrawGizmos()
        {
            Gizmos.color=Color.blue;
            for (int i = 1; i < _points.Count-1; i++)
            {
                Gizmos.DrawSphere(_points[i],0.1f);
            }
            
        }

        private void DoCurveAnimation()
        {
            
            for (int i = 1; i < _points.Count-1; i++)
            {
                Vector3 pos = Vector3.Lerp(_points[0], _points[_points.Count-1], (1f/_points.Count)*i);
                //pos += _setPoints[i];
               // _points[i] = pos;

                Vector3 dir  = (_points[i]-pos).normalized;
                 if (i % 2 == 0)
                     dir = Quaternion.Euler(Vector3.left*0.2f) * dir;
                 else
                     dir = Quaternion.Euler(Vector3.right*0.2f) * dir;
 
                 _points[i] = dir+pos;
            }
            
            
        }
        


        private Vector3 GetPointBezier(float t)
        {
            /*float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
        
            Vector3 p = uuu * _legStartPoint; 
            p += 3 * uu * t * p0; 
            p += 3 * u * tt * p3; 
            p += ttt * _legEndPoint; 
            return p;*/
            return Eval(t);
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

                    array[i] = GetPointBezier(t);
                }

                DoCurveAnimation();
                _lineRenderer.SetPositions(array);
                yield return null;
            }
        }
        

        
    }
}

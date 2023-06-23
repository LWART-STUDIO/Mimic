using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private LegCreator _legPrefab;
        private LegsSegment[] _legsSegments;
        private float _renderSpeed = 5f;
        private bool _mayUpdate = false;
        private int _segmentCount = 7;


        private void Awake()
        {
            _legsSegments = new LegsSegment[_segmentCount];
            for (int i = 0; i < _legsSegments.Length; i++)
            {
                _legsSegments[i] = new LegsSegment(3);
            }
            
        }

        private void Start()
        {
            for (int o = 0; o < _legsSegments.Length; o++)
            {
                Vector3 legPos = SetLegPosition();
                for (int i = 0; i < _legsSegments[o].Leg.Length; i++)
                {
                    LegCreator leg = Instantiate(_legPrefab);
                    _legsSegments[o].Leg[i] = leg;
                    _legsSegments[o].Leg[i].SetBodyPosition(transform.position);
                    _legsSegments[o].Leg[i].SetFootPosition(legPos);
                    _legsSegments[o].Leg[i].SetPoints();
                    StartCoroutine(DrawLeg(_legsSegments[o].Leg[i]));
                }
            }
            

            _mayUpdate = true;
        }

        private void Move()
        {
            float horizontalInput=Input.GetAxis("Horizontal");
            float verticalInput=Input.GetAxis("Vertical");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down,out hit,Mathf.Infinity))
            {
            }
            Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
            movementDirection.Normalize();
            transform.Translate(movementDirection*4f*Time.deltaTime,Space.World);
            transform.position = new Vector3(transform.position.x, hit.point.y+2f, transform.position.z);
            if (movementDirection != Vector3.zero)
            {
                transform.forward = movementDirection;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position+transform.forward*5f,3f);
        }

        private Vector3 SetLegPosition()
        {
            Vector3 rayPosition = Random.insideUnitCircle*3f;
            rayPosition = new Vector3(rayPosition.x, 0, rayPosition.y);
            rayPosition += transform.position+transform.forward*5f;
            RaycastHit hit;
            if (Physics.Raycast(rayPosition, Vector3.down,out hit,Mathf.Infinity))
            {
                return  hit.point;
            }
            else
              return new Vector3(0,0,0);

        }

        private void Update()
        {
            Move();
            if(!_mayUpdate)
                return;
            for (int o = 0; o < _legsSegments.Length; o++)
            {
                for (int i = 0; i < _legsSegments[o].Leg.Length; i++)
                {
                    var legs = _legsSegments[o].Leg[i];
                    legs.SetBodyPosition(transform.position);
                    
                }
                UpdateLegPosition(_legsSegments[o]);
            }

           
        }

        private void UpdateLegPosition(LegsSegment legsSegment)
        {
            if (!legsSegment.Leg[0].LegTooLong(transform))
                    return;
                StartCoroutine(HideLegs(legsSegment));
                
        }

        private IEnumerator HideLegs( LegsSegment legsSegment)
        {
            Vector3 legPos = SetLegPosition();
            for (int i = 0; i < legsSegment.Leg.Length; i++)
            {
                StartCoroutine(HideLeg(legsSegment.Leg[i], legPos));
                yield return null;
            }
        }

        private IEnumerator HideLeg(LegCreator legCreator,Vector3 legPos)
        {
            legCreator.LockLeg();
            float startTime = Time.time;
            while (true)
            {
                float distCovered = (Time.time - startTime) * _renderSpeed;
                float fractionOfJourney = distCovered / 1f;
                legCreator.SetUpRenderValue(Mathf.Lerp(1f,0.001f,fractionOfJourney));
                if(fractionOfJourney>1.1f)
                    break;
                yield return null;
            }
            legCreator.StopMove();
            yield return null;
            if(legPos!=Vector3.zero)
                legCreator.SetFootPosition(legPos);
            legCreator.SetPoints();
           
            StartCoroutine(DrawLeg(legCreator));
        }
        private IEnumerator DrawLeg( LegCreator legCreator)
        {
            legCreator.SetUpRenderValue(0.001f);
            legCreator.StartMove();
            float startTime = Time.time;
            while (true)
            {
                float distCovered = (Time.time - startTime) * _renderSpeed;
                float fractionOfJourney = distCovered / 1f;
                legCreator.SetUpRenderValue(Mathf.Lerp(0.001f,1f,fractionOfJourney));
                if(fractionOfJourney>1.1f)
                    break;
                yield return null;
            }
            
        }
    }

    [Serializable]
    public struct LegsSegment
    {
        public LegCreator[] Leg;

        public LegsSegment(int count)
        {
            Leg = new LegCreator[count];
        }
    }
}

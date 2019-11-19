using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBurst : MonoBehaviour {
    public bool isMoving = true;
    Vector3 destination;

    bool isTimeOut;
    float elapsedTime;
    int particleCount;
    ParticleSystem pSys;
    ParticleSystem.Particle[] m_Particles;
    // Use this for initialization
    void Start () {
        elapsedTime = 0;
        isTimeOut = false;
        pSys = GetComponent<ParticleSystem>();
    }
    void OnEnable()
    {
        elapsedTime = 0;
        isMoving = false;
        isTimeOut = false;
    }
	
	// Update is called once per frame
	void LateUpdate() {
        if (isTimeOut)
        {
            gameObject.Recycle();
            isTimeOut = false;
            return;
        }
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.35f)
            isMoving = true;
        else if (elapsedTime > 2.2f)
            isTimeOut = true;

        if (isMoving)
        {
            if (pSys == null)
            {
                isTimeOut = true;
                return;
            }

            InitializeIfNeeded();

            if (m_Particles.Length < 1)
            {
                isTimeOut = true;
                return;
            }

            int numParticlesAlive = pSys.GetParticles(m_Particles);


            //Vector3 target = Vector3.zero;
            var target = destination - pSys.transform.position;

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                //m_Particles[i].position = Vector3.MoveTowards(m_Particles[i].position, target, Time.deltaTime * 4);
                //m_Particles[i].velocity = (Vector3.zero - m_Particles[i].position).normalized;
                //m_Particles[i].velocity = Vector3.Lerp(m_Particles[i].velocity, (destination - m_Particles[i].position).normalized * speed, Time.deltaTime / 5.0f);
                //m_Particles[i].position += (destination - m_Particles[i].position) / (m_Particles[i].remainingLifetime) * Time.deltaTime;
                m_Particles[i].velocity = Vector3.zero;
                // Interpolate the movement towards the target with a nice quadratic easing					
                m_Particles[i].position = Vector3.Lerp(m_Particles[i].position, target, Time.deltaTime / m_Particles[i].remainingLifetime);

            }

            // Apply the particle changes to the particle system
            pSys.SetParticles(m_Particles, numParticlesAlive);
        }
    }

    //void Update()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    }
    //}
    void InitializeIfNeeded()
    {

        if (m_Particles == null || m_Particles.Length < pSys.main.maxParticles)
        {
            m_Particles = new ParticleSystem.Particle[pSys.main.maxParticles];
        }
    }
    public void SetDestination(Vector2 des)
    {
        destination = des;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}

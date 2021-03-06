
/**
* Copyright (c) 2010-2015, WyrmTale Games and Game Components
* All rights reserved.
* http://www.wyrmtale.com
*
* THIS SOFTWARE IS PROVIDED BY WYRMTALE GAMES AND GAME COMPONENTS 'AS IS' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL WYRMTALE GAMES AND GAME COMPONENTS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR 
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using UnityEngine;

/// <summary>
/// Die base class to determine if a die is rolling and to calculate it's current value
/// </summary>
public class Die : MonoBehaviour
{

	//------------------------------------------------------------------------------------------------------------------------------
	// public attributes
	//------------------------------------------------------------------------------------------------------------------------------

	// current value, 0 is undetermined (die is rolling) or invalid.
	public int value = 0;
	public ParticleSystem dieParticle;

	//------------------------------------------------------------------------------------------------------------------------------
	// private attributes
	//------------------------------------------------------------------------------------------------------------------------------	

	// normalized (hit)vector from die center to upper side in local space is used to determine what side of a specific die is up/down = value
	private Vector3 localHitNormalized;
	// hitVector check margin
	private readonly float validMargin = 0.02F;
	private float maxSpeed = 1f;
	private bool isOnGround = false;
	
	private Rigidbody rigid;

	void Awake()
	{
		this.rigid = this.GetComponent<Rigidbody>();
		this.localHitNormalized = Vector3.zero;
		this.isOnGround = false;
	}

	// true is die is still rolling
	public bool rolling
	{
		get
		{
			if (this.rigid == null)
				return false;

			return !(this.rigid.velocity.sqrMagnitude < .005F && this.rigid.angularVelocity.sqrMagnitude < .005F);
		}
	}

	// calculate the normalized hit vector and should always return true
	public bool localHit
	{
		get
		{
			if (rolling || isOnGround == false)
				return false;

			float rayDistance = 0.02f;// transform.lossyScale.z;
			// create a Ray from straight above this Die , moving downwards
			Ray ray = new Ray(transform.position + (new Vector3(0, 0, -rayDistance*2)/* * transform.localScale.sqrMagnitude */), Vector3.forward);
			RaycastHit hit = new RaycastHit();
			Debug.DrawRay(ray.origin, ray.direction * rayDistance);
			// cast the ray and validate it against this die's collider
			if (GetComponent<Collider>().Raycast(ray, out hit, rayDistance))
			{
				if (hit.transform.gameObject.name != this.gameObject.name)
					return false;

				// we got a hit so we determine the local normalized vector from the die center to the face that was hit.
				// because we are using local space, each die side will have its own local hit vector coordinates that will always be the same.
				localHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
				//Debug.Log(this.gameObject.name + " : " + localHitNormalized);
				return true;
			}
			// in theory we should not get at this position!
			return false;
		}
	}

	// calculate this die's value
	void GetValue()
	{
		// value = 0 -> undetermined or invalid
		if (value > 0)
			return;

		float delta = 1;
		// start with side 1 going up.
		int side = 1;
		Vector3 testHitVector;
		// check all sides of this die, the side that has a valid hitVector and smallest x,y,z delta (if more sides are valid) will be the closest and this die's value
		do
		{
			// get testHitVector from current side, HitVector is a overriden method in the dieType specific Die subclass
			// eacht dieType subclass will expose all hitVectors for its sides,
			testHitVector = HitVector(side);
			if (testHitVector != Vector3.zero)
			{
				// this side has a hitVector so validate the x,y and z value against the local normalized hitVector using the margin.
				if (valid(localHitNormalized.x, testHitVector.x) &&
					valid(localHitNormalized.y, testHitVector.y) &&
					valid(localHitNormalized.z, testHitVector.z))
				{
					float angle = Mathf.Abs(Vector3.Angle(localHitNormalized, testHitVector));

					if (angle < 2f)
					{
						// this side is valid within the margin, check the x,y, and z delta to see if we can set this side as this die's value
						// if more than one side is within the margin (especially with d10, d12, d20 ) we have to use the closest as the right side
						//float nDelta = Mathf.Abs(localHitNormalized.x - testHitVector.x) + Mathf.Abs(localHitNormalized.y - testHitVector.y) + Mathf.Abs(localHitNormalized.z - testHitVector.z);
						//if (nDelta < validMargin)
						{
							value = side;
							//Debug.LogWarning(this.gameObject.name + "'s side : " + side);// + " and dot : " + dot);
							this.rigid.velocity = Vector3.zero;
							break;
						//delta = nDelta;
						}
					}
				}
			}
			// increment side
			side++;
			// if we got a Vector.zero as the testHitVector we have checked all sides of this die
		} while (side <= 6);
	}
	
	//void FixedUpdate()
	//{
	//	if (rolling)
	//	{
	//		if (this.rigid.velocity.magnitude > maxSpeed)
	//		{
	//			this.rigid.velocity = this.rigid.velocity.normalized * maxSpeed;
	//			Debug.Log("Die : Over the limit.");
	//		}
	//	}
	//}

	//void Update()
	//{
	//	// determine the value is the die is not rolling
	//	if (localHit && isOnGround && this.value <= 0)
	//	{
	//		GetValue();

	//		if (this.value <= 0)
	//		{
	//			ReRoll();
	//			return;
	//		}
	//	}
	//}

	private void ReRoll()
	{
		Rigidbody rigid = this.rigid;
		if (rigid == null)
			return;

		if (rigid.useGravity == false)
			return;

		float torqueForce = 5f;
		rigid.AddTorque(new Vector3(Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce)), ForceMode.Impulse);

		float force = .1f;
		rigid.AddForce(new Vector3(Random.Range(-force, force), Random.Range(-force, force), force), ForceMode.Impulse);
		NGUIDebug.Log("Reroll");
	}

	void OnSollisionExit(Collision collision)
	{
		if (collision.gameObject.name == "platform")
		{
			isOnGround = false;
		}
	}

	void OnCollisionStay(Collision collision)
	{
		if (isOnGround)
		{
			if (localHit && this.value <= 0)
			{
				GetValue();
				if (this.value <= 0)
				{
					ReRoll();
					isOnGround = false;
				}
			}
			return;
		}
		
		if (collision.gameObject.name == "platform")
		{
			isOnGround = true;
			return;
		}

		if (this.rolling == false)
			return;

		if (collision.transform.parent == null)
			return;

		if (this.rigid.velocity.magnitude > (maxSpeed + .2f))
		{
			this.rigid.velocity = this.rigid.velocity.normalized * maxSpeed;
			Debug.Log("Die : Over the limit.");
		}
		else
		{
			// 서로 반대 방향으로 튕겨낸다.
			Vector3 direction = collision.transform.forward;
			if (collision.gameObject.name.Contains("up"))
			{
				direction = -collision.transform.up;
			}
			else if(collision.gameObject.name.Contains("down"))
			{
				direction = collision.transform.up;
			}
			else if(collision.gameObject.name.Contains("left"))
			{
				direction = collision.transform.right;
			}
			else if(collision.gameObject.name.Contains("right"))
			{
				direction = -collision.transform.right;
			}

			Vector3 v = Vector3.Reflect(this.rigid.velocity.normalized, direction); // rigidbody.velocity.normalized;
			float force = .1f;
			rigid.AddForce(v * force, ForceMode.Impulse);
			float torqueForce = 5f;
			rigid.AddTorque(new Vector3(Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce)), ForceMode.Impulse);
		}
	}

	private Vector3 Force()
	{
		float force = 1f;
		int isPlusSign = Random.Range(0, 2);
		if(isPlusSign == 0)
		{
			force *= -1;
		}
		return new Vector3(Random.Range(1f, 2f) * force, Random.Range(1f, 2f) * force, -2f);
	}

	// validate a test value against a value within a specific margin.
	private bool valid(float t, float v)
	{
		if (t > (v - validMargin) && t < (v + validMargin))
			return true;
		else
			return false;
	}

	// virtual  method that to get a die side hitVector.
	// this has to be overridden in the dieType specific subclass
	private Vector3 HitVector(int side)
	{
		switch (side)
		{
			case 1: return new Vector3(0F, 0F, 1F);
			case 2: return new Vector3(0F, -1F, 0F);
			case 3: return new Vector3(-1F, 0F, 0F);
			case 4: return new Vector3(1F, 0F, 0F);
			case 5: return new Vector3(0F, 1F, 0F);
			case 6: return new Vector3(0F, 0F, -1F);
		}
		return Vector3.zero;
	}
}

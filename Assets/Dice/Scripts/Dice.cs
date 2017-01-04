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
using System.Collections;
using ToBeFree;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This dice dupporting class has some 'static' methods to help you throwning dice
///  and getting the rolling dice count, value or rolling state (asString)
/// </summary>
public class Dice : MonoBehaviour {

	//------------------------------------------------------------------------------------------------------------------------------
	// public attributes
	//------------------------------------------------------------------------------------------------------------------------------

	// rollSpeed determines how many seconds pass between rolling the single dice
	public float rollSpeed = 0.25F;
	
	//------------------------------------------------------------------------------------------------------------------------------
	// protected and private attributes
	//------------------------------------------------------------------------------------------------------------------------------

	// keep rolling time to determine when dice to be rolled, have to be instantiated
	protected float rollTime = 0;
	
	// material cache
	private ArrayList matNames = new ArrayList();
	private ArrayList materials = new ArrayList();

	// reference to the dice that have to be rolled
	private ArrayList rollQueue = new ArrayList();
	
	// reference to all dice, created by Dice.Roll
	private ArrayList allDice = new ArrayList();
	// reference to the dice that are rolling
	private ArrayList rollingDice = new ArrayList();
	
	[SerializeField]
	private Transform spawnPoint = null;
	[SerializeField]
	private UILabel nameLabel;
	[SerializeField]
	private UILabel statLabel;
	
	[SerializeField]
	private UILabel diceNumLabel;

	private float positionX;
	private float positionY;
	

	//------------------------------------------------------------------------------------------------------------------------------
	// public methods
	//------------------------------------------------------------------------------------------------------------------------------	

	public void Freeze(bool isFreeze)
	{
		foreach (var dice in allDice)
		{
			RollingDie rollingDie = dice as RollingDie;

			rollingDie.SetGravity(!isFreeze);

			if(isFreeze)
				rollingDie.force = Vector3.zero;
			else
			{
				rollingDie.gameObject.GetComponent<Collider>().enabled = true;
				rollingDie.gameObject.GetComponent<Rigidbody>().AddForce(Force(), ForceMode.Impulse);
				// apply a random torque
				rollingDie.gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(-50 * Random.value, -50 * Random.value, -50 * Random.value), ForceMode.Impulse);
				//rollingDie.gameObject.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
			}
		}
	}

	/// <summary>
	/// This method will create/instance a prefab at a specific position with a specific rotation and a specific scale and assign a material
	/// </summary>
	public GameObject prefab(string name, Vector3 position, Vector3 rotation, Vector3 scale, string mat)
	{		
		// load the prefab from Resources
		Object pf = Resources.Load("Prefabs/" + name);
		if (pf!=null)
		{
			// the prefab was found so create an instance for it.
			GameObject inst = (GameObject) GameObject.Instantiate( pf , Vector3.zero, Quaternion.identity);
			if (inst!=null)
			{
				// the instance could be created so set material, position, rotation and scale.
				if (mat!="") inst.GetComponent<Renderer>().material = material(mat);
				inst.transform.position = position;
				inst.transform.Rotate(rotation);
				inst.transform.localScale = scale;
				// return the created instance (GameObject)
				return inst;
			}
		}
		else
			Debug.Log("Prefab "+name+" not found!");
		return null;		
	}	
	
	/// <summary>
	/// This method will perform a quick lookup for a 'cached' material. If not found, the material will be loaded from the Resources
	/// </summary>
	public Material material(string matName)
	{
		Material mat = (Material) Resources.Load("Materials/"+matName);
		// return material - null if not found
		return mat;
	}

	/// <summary>
	/// Roll one or more dice with a specific material from a spawnPoint and give it a specific force.
	/// format dice 			: 	({count}){die type}	, exmpl.  d6, 4d4, 12d8 , 1d20
	/// possible die types 	:	d4, d6, d8 , d10, d12, d20
	/// </summary>
	public IEnumerator Init(int diceNum, string name, eTestStat stat = eTestStat.NULL)
	{
		if(spawnPoint == null)
		{
			spawnPoint = this.transform.FindChild("spawnPoint");
		}

		positionX = this.spawnPoint.position.x;
		positionY = this.spawnPoint.position.y;

		this.nameLabel.text = name;
		this.statLabel.text = stat.ToString();
		this.diceNumLabel.text = diceNum.ToString();

		string layerName = "Dice1";
		if(name == "Police")
		{
			layerName = "Dice2";
		}

		// instantiate the dice
		for (int d = 0; d < diceNum; d++)
		{
			yield return AddDie(LayerMask.NameToLayer(layerName));
		}
	}

	public IEnumerator AddDie(int layer)
	{
		string dieType = "d6";
		string mat = "red";

		Vector3 startPosition = new Vector3(0.5f, 19.5f);
		Vector3 destination = new Vector3(positionX, positionY, spawnPoint.position.z);
		
		// create the die prefab/gameObject
		GameObject die = prefab(dieType, startPosition, new Vector3(0f, 0f, -90f), new Vector3(0.2f, 0.2f, 0.2f), mat);
		die.layer = layer;
		
		// give it a random rotation
		//die.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
		// inactivate this gameObject because activating it will be handeled using the rollQueue and at the apropriate time
		//die.SetActive(false);
		// create RollingDie class that will hold things like spawnpoint and force, to be used when activating the die at a later stage
		RollingDie rDie = new RollingDie(die, dieType, mat);
		// add RollingDie to allDices
		allDice.Add(rDie);
		// add RollingDie to the rolling queue
		//rollQueue.Add(rDie);

		TweenPosition tweenPosition = die.AddComponent<TweenPosition>();
		tweenPosition.from = startPosition;
		tweenPosition.to = destination;
		tweenPosition.duration = 0.5f;
		tweenPosition.ignoreTimeScale = false;
		tweenPosition.PlayForward();

		Vector3 originScale = die.transform.localScale;
		TweenScale tweenScale = die.AddComponent<TweenScale>();
		tweenScale.from = Vector3.zero;
		tweenScale.to = originScale;
		tweenScale.duration = tweenPosition.duration;
		tweenScale.ignoreTimeScale = false;
		tweenScale.PlayForward();

		yield return new WaitForSeconds(tweenPosition.duration + 1);

		yield return null;

		positionX += 0.15f;
		if (allDice.Count % 3 == 0)
		{
			positionX = this.spawnPoint.position.x;
			positionY += 0.15f;
		}

		rDie.SetGravity(false);
		rDie.force = Vector3.zero;
	}

	/// <summary>
	/// Get value of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
	public int Value(string dieType)
	{
		int v = 0;
		// loop all dice
		for (int d = 0; d < allDice.Count; d++)
		{
			RollingDie rDie = (RollingDie)allDice[d];
			// check the type
			if (rDie.name == dieType || dieType == "")
				v += rDie.die.value;
		}
		return v;
	}

	/// <summary>
	/// Get number of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
	public int Count(string dieType)
	{
		int v = 0;
		// loop all dice
		for (int d = 0; d < allDice.Count; d++)
		{
			RollingDie rDie = (RollingDie)allDice[d];
			// check the type
			if (rDie.name == dieType || dieType == "")
				v++;
		}
		return v;
	}

	/// <summary>
	/// Get rolling status of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
	public string AsString(string dieType)
	{
		// count the dice
		string v = ""+Count(dieType);
		if (dieType == "")
			v += " dice | ";
		else
			v += dieType + " : ";
		
		if (dieType == "")
		{
			// no dieType specified to cumulate values per dieType ( if they are available )
			if (Count("d6") > 0) v += AsString("d6") + " | ";
			if (Count("d10") > 0) v += AsString("d10") + " | ";
		}
		else
		{
			// assemble status of specific dieType
			bool hasValue = false;
			for (int d = 0; d < allDice.Count; d++)
			{
				RollingDie rDie = (RollingDie)allDice[d];
				// check type
				if (rDie.name == dieType || dieType == "")
				{
					if (hasValue) v += " + ";
					// if the value of the die is 0 , no value could be determined
					// this could be because the die is rolling or is in a invalid position
					v += "" + ((rDie.die.value == 0) ? "?" : "" + rDie.die.value);
					hasValue = true;
				}
			}
			v += " = " + Value(dieType);
		}
		return v;
	}


	/// <summary>
	/// Clears all currently rolling dice
	/// </summary>
	public void Clear()
	{
		for (int d=0; d<allDice.Count; d++)
			GameObject.Destroy(((RollingDie)allDice[d]).gameObject);

		allDice.Clear();
		rollingDice.Clear();
		rollQueue.Clear();
	}

	public int GetSuccessNum(int standard)
	{
		int successNum = 0;
		foreach(RollingDie die in allDice)
		{
			if (die.value == 0)
				return -99;

			if(die.value >= standard)
			{
				successNum++;
			}
		}
		return successNum;
	}
	
	/// <summary>
	/// Check if there all dice have stopped rolling
	/// </summary>
	public bool IsRolling()
	{
		foreach(var dice in allDice)
		{
			RollingDie rollingDie = dice as RollingDie;
			if (rollingDie.rolling || rollingDie.value == 0)
				return true;
		}
		return false;
	}

	// dertermine random rolling force	
	private Vector3 Force()
	{
		float force = 1f;
		return new Vector3(Random.Range(-0.2f, 0.2f) * force, Random.Range(-0.2f, 0.2f) * force, -force * 2);
		Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
		return Vector3.Lerp(spawnPoint.position, rollTarget, 1).normalized * (-35 - Random.value * 5);
	}

	public void SetPosition(bool isPolice)
	{
		Vector3 position = Vector3.zero;
		float cameraX = 0f;
		if (isPolice==false)
		{
			position = new Vector3(400f, 0f, 0f);
			cameraX = -400f;
		}

		this.transform.localPosition = position;
		Camera diceCam = this.GetComponentInChildren<Camera>();
		if(diceCam)
		{
			Vector3 cameraPosition = diceCam.transform.localPosition;
			diceCam.transform.localPosition = new Vector3(cameraX, cameraPosition.y, cameraPosition.z);
		}
	}

	private IEnumerator StartEffect(int minSuccessNum)
	{
		foreach (RollingDie rollingDie in allDice.Cast<RollingDie>())
		{
			if (rollingDie.value >= minSuccessNum)
			{
				rollingDie.die.GetComponentInChildren<Light>().enabled = true;
				AudioManager.Instance.Find("success").Play();
				yield return new WaitForSeconds(1f);
			}
		}
	}

	public IEnumerator StartEffect(Dice dice, int minSuccessNum)
	{
		if(dice.gameObject.activeSelf == false)
		{
			yield return this.StartEffect(minSuccessNum);
			yield break;
		}

		List<Die> dieList1 = new List<Die>();
		foreach (RollingDie rollingDie in allDice.Cast<RollingDie>())
		{
			if (rollingDie.value >= minSuccessNum)
			{
				dieList1.Add(rollingDie.die);
			}
		}

		List<Die> dieList2 = new List<Die>();
		foreach (RollingDie rollingDie in dice.allDice.Cast<RollingDie>())
		{
			if (rollingDie.value >= minSuccessNum)
			{
				dieList2.Add(rollingDie.die);
			}
		}

		int count = dieList1.Count;
		if (count < dieList2.Count)
		{
			count = dieList2.Count;
		}

		for (int i = 0; i < count; ++i)
		{
			if(i < dieList1.Count)
				dieList1[i].GetComponentInChildren<Light>().enabled = true;
			if (i < dieList2.Count)
				dieList2[i].GetComponentInChildren<Light>().enabled = true;

			AudioManager.Instance.Find("success").Play();
			yield return new WaitForSeconds(1f);
		}
	}	
}

/// <summary>
/// Supporting rolling die class to keep die information
/// </summary>
class RollingDie
{

	public GameObject gameObject;		// associated gameObject
	public Die die;								// associated Die (value calculation) script

	public string name = "";				// dieType
	public string mat;						// die material (asString)
	public Vector3 force;					// die initial force impuls

	// rolling attribute specifies if this die is still rolling
	public bool rolling
	{
		get
		{
			return die.rolling;
		}
	}

	public bool localHit
	{
		get
		{
			return die.localHit;
		}
	}

	public int value
	{
		get
		{
			return die.value;
		}
	}

	// constructor
	public RollingDie(GameObject gameObject, string name, string mat)
	{
		this.gameObject = gameObject;
		this.name = name;
		this.mat = mat;

		// get Die script of current gameObject
		die = gameObject.GetComponent<Die>();
	}

	public void SetGravity(bool useGravity)
	{
		die.GetComponent<Rigidbody>().useGravity = useGravity;
		//die.GetComponent<Rigidbody>().isKinematic = !useGravity;
		//die.GetComponent<Collider>().enabled = useGravity;
	}
}
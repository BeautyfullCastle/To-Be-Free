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

// Demo application script
public class AppDemo : MonoBehaviour
{
	public int diceNum = 0;

	// initial/starting die in the gallery
	private string galleryDie = "d6-red";
	
	private Rect rectModeSelect;
	
	// Use this for initialization
	void Start ()
	{
		rectModeSelect =  new Rect(10,10,180,80);
		Dice.Clear();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// rolling mode to update the dice rolling
		UpdateRoll();
	}

	// dertermine random rolling force
	private GameObject spawnPoint = null;
	private Vector3 Force()
	{
		Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
		return Vector3.Lerp(spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
	}

	bool mouseDown = false;
	void UpdateRoll()
	{
		spawnPoint = GameObject.Find("spawnPoint");
		
		// check if we have to roll dice
		if (Input.GetMouseButtonDown(0) && !PointInRect(GuiMousePosition(), rectModeSelect))
		{
			if(mouseDown == false)
			{
				Dice.Clear();
				mouseDown = true;
			}	
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(mouseDown)
			{
				string[] a = galleryDie.Split('-');
				if (diceNum <= 0)
				{
					diceNum = 1;
				}

				Dice.Roll(diceNum.ToString() + a[0], galleryDie, spawnPoint.transform.position, Force());
				
				mouseDown = false;
			}
		}
	}

	// check if a point is within a rectangle
	private bool PointInRect(Vector2 p, Rect r)
	{
		return  (p.x>=r.xMin && p.x<=r.xMax && p.y>=r.yMin && p.y<=r.yMax);
	}

	// translate Input mouseposition to GUI coordinates using camera viewport
	private Vector2 GuiMousePosition()
	{
		Vector2 mp = Input.mousePosition;
		Vector3 vp = Camera.main.ScreenToViewportPoint(new Vector3(mp.x, mp.y, 0));
		mp = new Vector2(vp.x * Camera.main.pixelWidth, (1 - vp.y) * Camera.main.pixelHeight);
		return mp;
	}
}


using System;
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
	public Dice[] dices = null;

	[HideInInspector]
	public bool mouseDown = false;
		
	private Rect rectModeSelect;

	public void Init(int characterDiceNum, int policeDiceNum = 0)
	{
		dices[0].Clear();
		dices[1].Clear();

		if (characterDiceNum <= 0)
			characterDiceNum = 1;
		
		dices[0].Init(characterDiceNum);
		dices[1].Init(policeDiceNum);

		bool isPolice = policeDiceNum > 0;
		dices[1].gameObject.SetActive(isPolice);
	}

	void Awake()
	{
		rectModeSelect = new Rect(10, 10, 180, 80);

		foreach (Dice dice in dices)
		{
			dice.Clear();
		}

		this.gameObject.SetActive(false);
	}

	void OnDisable()
	{
		foreach (Dice dice in dices)
		{
			dice.Clear();
		}
		mouseDown = false;
	}

	// Update is called once per frame
	void Update ()
	{
		// rolling mode to update the dice rolling
		UpdateRoll();
	}
	
	void UpdateRoll()
	{
		//check if we have to roll dice
		if (Input.GetMouseButtonDown(0) && !PointInRect(GuiMousePosition(), rectModeSelect))
		{
			if (mouseDown == false)
			{
				mouseDown = true;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if(mouseDown)
			{
				foreach (Dice dice in dices)
				{
					dice.rolling = true;
					dice.Freeze(false);
				}
				
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


using System;
using System.Collections;
using ToBeFree;
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

	[SerializeField]
	private RollButton button;

	[HideInInspector]
	public bool mouseDown = false;

	private eTestStat stat;

	public IEnumerator Init(eTestStat stat, int characterDieNum, int policeDieNum = 0)
	{
		if (characterDieNum <= 0)
			characterDieNum = 1;

		bool hasPolice = policeDieNum > 0;
		dices[0].SetPosition(hasPolice);
		dices[1].gameObject.SetActive(hasPolice);

		dices[0].Init(characterDieNum, false, GameManager.Instance.Character.Name, stat);
		dices[1].Init(policeDieNum, true, "Police", stat);

		yield return dices[0].InitDies(characterDieNum, false);
		yield return dices[1].InitDies(policeDieNum, true);

		button.SetEnable(true);
		this.stat = stat;

		Stat.OnValueChange += Stat_OnValueChange;
	}

	void Awake()
	{
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
	
	public void OnButtonClick()
	{
		foreach (Dice dice in dices)
		{
			dice.Freeze(false);
		}
		mouseDown = true;
		Stat.OnValueChange -= Stat_OnValueChange;
		
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

	// 스탯 장착템으로 인해 불러오는 이벤트.
	private void Stat_OnValueChange(int value, eStat stat)
	{
		if (EnumConvert<eTestStat>.ToString(this.stat) == EnumConvert<eStat>.ToString(stat))
		{
			int characterStatNum = GameManager.Instance.Character.GetDiceNum(this.stat);
			StartCoroutine(this.AddDie());
		}
	}

	public IEnumerator AddDie()
	{
		while(true)
		{
			if (dices[0].AddingDie)
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				break;
			}
		}

		dices[0].AddDieNum();
		yield return dices[0].AddDie(LayerMask.NameToLayer("Dice1"));
	}
}

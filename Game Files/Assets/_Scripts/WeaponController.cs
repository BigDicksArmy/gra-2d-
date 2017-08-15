﻿using System.Collections;
using UnityEngine;
public delegate void WeaponAction();
public class WeaponController : MonoBehaviour
{
	public static event WeaponAction OnShot;
	public GameObject bulletSpawn;
	public WeaponStats stats;
	public AudioClip clip;

	private EquipmentController equipment;
	private Transform weaponPlace;
	private Transform spawn;
	private Camera mainCamera;

	private float timeToFire = 0f;
	private float triggerPullTime = 0.1f;

	private void Awake()
	{
		mainCamera = Camera.main;
		weaponPlace = GetComponent<Transform>();
		equipment = transform.parent.GetComponent<EquipmentController>();
	}
	private void Start()
	{
		stats = EquipmentController.Instance.Current.Firearm.GetComponent<WeaponController>().stats; // null ref
		stats.firemode = FireMode.Single;
		spawn = weaponPlace.Find("BulletSpawn");
		spawn.localPosition = EquipmentController.Instance.Current.Firearm.transform.localPosition + bulletSpawn.transform.localPosition;
	}

	void FixedUpdate()
	{
		switch (stats.firemode)
		{
			case FireMode.Single:
				SingleShot();
				break;
			case FireMode.Automatic:
				AutoShot();
				break;
			default:
				Debug.Log("Fire mode not choosen - not good");
				break;
		}
		if (Input.GetKeyDown(KeyCode.R) && stats.BulletsLeft >= 0 && stats.BulletsLeft <= stats.MagazineSize)
		{
			StartCoroutine(Reload(stats.ReloadSpeed));
		}
		if (Input.GetKeyDown(KeyCode.V) && stats.IsAutomatic)
		{
			//If AutoFire was on then switch to singleFire else switch to autoFire
			stats.firemode = stats.firemode == FireMode.Automatic ? stats.firemode = FireMode.Single : stats.firemode = FireMode.Automatic;
		}
	}

	#region Shooting Functions
	private void Shot()
	{
		RaycastHit2D raycastInfo = Physics2D.Raycast(spawn.position, CalculateDirection());
		if (OnShot!=null)
		{
			print("OnShot event triggered");
			OnShot();
		}
		
	}

	private Vector2 CalculateDirection() //decided not to mess with the dot product of vectors
	{
		Vector2 symmetryLine = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float Bounduary = symmetryLine.magnitude * Mathf.Tan(Mathf.Deg2Rad * stats.BulletSpread / 2);
		float yCoord = UnityEngine.Random.Range(-Bounduary, Bounduary);
		Vector2 direction = new Vector2(symmetryLine.x, symmetryLine.y + yCoord);
		return direction;
	}
	IEnumerator Reload(float time)
	{
		if (stats.MagazineCount > 0 && stats.BulletsLeft < stats.MagazineSize)
		{
			yield return new WaitForSeconds(time);
			stats.BulletsLeft = stats.MagazineSize;
			stats.MagazineCount--;
		}
	}
	void SingleShot()
	{
		if (Input.GetButtonDown("Fire1") && stats.BulletsLeft > 0 && Time.time > timeToFire)
		{
			timeToFire = Time.time + triggerPullTime;
			Shot();
			stats.BulletsLeft--;
		}
	}
	void AutoShot()
	{
		if (Input.GetButton("Fire1") && stats.BulletsLeft > 0 && Time.time > timeToFire)
		{
			timeToFire = Time.time + 1 / stats.FireRate + triggerPullTime;
			Shot();
			stats.BulletsLeft--;
		}
	}
	#endregion
}

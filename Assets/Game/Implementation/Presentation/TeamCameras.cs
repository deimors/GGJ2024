using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCameras : IEnumerable<Camera>
{
	private readonly List<Camera> _cameras = new();

	public void Add(Camera camera) 
		=> _cameras.Add(camera);

	public IEnumerator<Camera> GetEnumerator() 
		=> _cameras.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() 
		=> ((IEnumerable)_cameras).GetEnumerator();
}
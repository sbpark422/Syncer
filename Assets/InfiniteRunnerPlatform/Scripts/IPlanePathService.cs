using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xeemu.PathAutoGen
{
	public interface IPlanePathService 
	{
		//Helping in destroy Paths
		void DestroyProcess(ref float playerPositionZ,ref PlayerEnteringPathModel playerEnteringPathModel,ref PathGameObjectModel pathGameObjectModel,ref RigidBodyModel rigidBodyModel,
			ref PathModel pathSize, ref int pathNum, ref int rarePathCount, ref int rarePathRepeatAfterNoOfPath, ref GameObject rarePath, ref List<GameObject> pathList,int destroyPathNum,
			PathController coinController,float oneunitySize,GameObject coins, float maxCoinHeight ,int _noOfSize,
			ref ObstacleModel obstacleModel,ref float speed);

		//helping in Generateing Path	
		void GeneratePath ( ref PathPositionModel pathPositionModel,ref PathGameObjectModel pathGameObjectModel,ref List<GameObject> pathList,ref RigidBodyModel rigidBodyModel, ref PathModel pathSize,
			PathController coinController,float oneunitySize,GameObject coins,float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstacleModel,ref float speed);	



	}
}
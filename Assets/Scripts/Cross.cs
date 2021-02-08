﻿using UnityEngine;
using System.Collections.Generic;

//路口实例为一个大型的平面透明物体，负责处理车辆在道路之间的调度
//路口类需要根据路口进入车流量对车辆进行概率上的分发（确定车辆进入车道与驶出车道）
//红绿灯处理逻辑也放在该类中
//LineIn增加驶入准许，RoadOut增加驶出准许
//红绿灯配时设计

//车辆调用链:进入路口区域->选择想要驶出的道路（权重随机）-> 选择能够到达驶出道路的Line并行驶 -> 在目标Road中选择一条目标Line（随机选择或是选择价值系数最优）
//->由所在Line与目标Line算出行驶曲线

//需要注意的是，在路口中并不允许换道超车等行为，于是其中只有跟驰与避让行为，避让可以在车辆前进路线上设置一虚拟车辆使其跟驰。

//车辆流量的设置，采用交叉口流量设置的话，是在每一个RoadIn设置到每一个方向Road的流量，故对于RoadIn->RoadOut的映射需要附带一个流量数据集
public class Cross : MonoBehaviour
{
    LinkedList<Car> cars;

    Dictionary<Road, RoadOut> RoadMap;

    //维护LineOut对应的交通配时方案
    //Dictionary<Line, LightTimeSet> LightSet;

    //Dictionary<Line, Road[]> Line2Out;
    //OutLine交通灯状态
    //Light[] Light;
    
    //每一个LineOut对应的可驶入的LineIn

    //车辆进入路口区域触发
    void OnTriggerEnter(Collider other)
    {
        //other is not a car
        if (other.gameObject.GetComponent<Car>() == null) {
            return;
        }
        Car car = other.gameObject.GetComponent<Car>();
        cars.AddLast(car);
        car.state = Car.State.prepareCross;
    }

    //根据道路车流量权重随机选择驶出道路
    public Road FindRoadOut(Car car)
    {
        RoadOut roadOut = RoadMap[car.line.fatherRoad];
        float rand = Random.Range(0,roadOut.totalCars);
        for (int i = 0,temp = 0; i < roadOut.carstream.Length; i++)
        {
            temp += roadOut.carstream[i];
            if (rand <= temp)
            {
                return roadOut.roadOuts[i];
            }
        }
        Debug.LogError("RoadIn streams set error !");
        return null;
    }

    public Line FindLineIn(Car car,Road roadOut)
    {
        Road roadIn = car.line.fatherRoad;
        //循环遍历Car行驶Road中的每一条Line，判断是否能够行驶到roadOut
        for (int i=0; i < roadIn.lines.Length; i++)
        {
            //判断起始位置从当前车道开始
            int j = (i + car.line.indexInRoad()) % roadIn.lines.Length;
            foreach(Road nextRoad in roadIn.lines[j].nextRoads)
            {
                if(nextRoad == roadOut)
                {
                    return roadIn.lines[j];
                }
            }
        }
        Debug.LogError("Cross.FindLineIn error");
        return null;
    }

    //从已经选择好的roadOut中选择一条“较好”的道路
    public Line FindLineOut(Road roadOut)
    {
        //暂时选择随机选取
        return roadOut.lines[Random.Range(0, roadOut.lines.Length)];
    }

    void Start()
    {
        cars = new LinkedList<Car>();
    }

    void Update()
    {

    }
}
class RoadOut
{
    public Road[] roadOuts;
    public int[] carstream;
    public int totalCars;
}
/*public enum CrossLight
{
    RED,
    YELLOW,
    GREEN
}

class LightTimeSet
{
    float red, yellow, green;
}*/
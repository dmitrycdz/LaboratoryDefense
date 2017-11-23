﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ImmediateCube : AttackCube
{
    public GameObject target;
    public GameObject hitEffect;

    public void Generate(ImmediateCube source)
    {
        this.damage = source.damage;
        this.attackType = source.attackType;
        this.targetType = source.targetType;
        this.bulletPrefab = source.bulletPrefab;
        this.ballisticVelocity = source.ballisticVelocity;
        this.isTracing = source.isTracing;
        this.target = source.target;
        this.hitEffect = source.hitEffect;
    }

    public override string ToString()
    {
        StringBuilder ans = new StringBuilder();
        ans.Append("Normal Attack:\n");
        ans.Append(base.ToString());
        return ans.ToString();
    }
}
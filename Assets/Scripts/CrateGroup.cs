using UnityEngine;
using System;
using System.Collections.Generic;

public class CrateGroup {
  
   public List<Crate> crates;
   public List<Vector3> spins = new List<Vector3>();
   // spins stores (x, y, dir); where xy is the axis of rotation and the dir is +1 (positive rotation/CCW) or -1 (neg, CW) 
   public Vector2 pusherForce = Vector2.zero;
   public Vector2 netForce = Vector2.zero;
   public Vector2 lastDirection = Vector2.zero;

   public void ApplyForces() {
      if (spins.Count != 0) {
         //TODO: NEEDS TO CHANGE TO SOMEHOW CHOOSE BETWEEN ROTATIONS IF MULTIPLE
         TryRotation(spins[0]);
      } else if (pusherForce != Vector2.zero) {
         TryMoveFromForce(pusherForce);
      } else if (netForce != Vector2.zero) {
         TryMoveFromForce(netForce);
      } else {
         lastDirection = Vector2.zero;
      }
   }

   public void TryRotation(Vector3 spin) {
      Debug.Log("TryRotation--------------");
      
      if (CanRotate(spin)) Rotate(spin);
   }

   public bool CanRotate(Vector3 spin) {
      Debug.Log("CanRotate--------------");
      foreach (Crate crate in crates) {
         Debug.Log(crate.xy);
         if (!crate.CanRotate(spin)) return false;
      }
      return true;
   }

   public void Rotate(Vector3 spin) {
      foreach (Crate crate in crates) {
         crate.Rotate(spin);
      }
   }
   public void TryMoveFromForce(Vector2 force) {
      Vector2[] directions = DirectionsFromVectorForce(force);
      bool canMovePrimaryDir = directions[0] != Vector2.zero && CanMove(directions[0]);
      bool canMoveSecondaryDir = directions[1] != Vector2.zero && CanMove(directions[1]);
      if (canMovePrimaryDir) {
         Move(directions[0]);
      } else if (canMoveSecondaryDir) {
         Move(directions[1]);
      } else {
         lastDirection = Vector2.zero;
      }
   }

   public bool CanMove(Vector2 direction) {
      foreach (Crate crate in crates) {
         if (!crate.CanMove(direction)) return false;
      }
      return true;
   }

   public void Move(Vector2 direction) {
      foreach (Crate crate in crates) {
         crate.Move(direction);
      }
      lastDirection = direction;
   }

   public Vector2[] DirectionsFromVectorForce(Vector2 force) {
      Vector2[] dirs = new Vector2[] {Vector2.zero, Vector2.zero};
      if (force == Vector2.zero) return dirs;
      Vector2 xPart = new Vector2(force.x,0);
      Vector2 yPart = new Vector2(0,force.y);
      
      if (Math.Abs(force.x) == Math.Abs(force.y)) force += lastDirection;
      if (Math.Abs(force.x) > Math.Abs(force.y)) {
         dirs[0] = xPart / xPart.magnitude;
         dirs[1] = yPart / yPart.magnitude;
      } else {
         dirs[0] = yPart / yPart.magnitude;
         dirs[1] = xPart / xPart.magnitude;
      } 
      return dirs;
   }

   public void OnStepStart() {
      spins.Clear();
      pusherForce = Vector2.zero;
      netForce = Vector2.zero;

   }

   public CrateGroup() {
      crates = new List<Crate>();
   }
}
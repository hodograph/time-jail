var playerObject : GameObject; // the player
var fieldOfViewRange : float; // in degrees (I use 68, this gives the enemy a vision of 136 degrees)
var minPlayerDetectDistance : float; // the distance the player can come behind the enemy without being deteacted
var rayRange : float; // distance the enemy can "see" in front of him
var rayAngle : float; //zero angle for guard to look in.
private var rayDirection = Vector3.zero;

function CanSeePlayer() : boolean
{
     var hit : RaycastHit;
     var rayDirection = playerObject.transform.position - transform.position;
     var distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
 
     if(Physics.Raycast (transform.position, rayDirection, hit))
	{ // If the player is very close behind the player and in view the enemy will detect the player
         if((hit.transform.tag == "Player") && (distanceToPlayer <= minPlayerDetectDistance))
         {
         	return true;
         }
	}
	Debug.DrawRay(transform.position, rayDirection);
	//Debug.Log(Vector3.Angle(rayDirection, transform.forward));
	 if((Vector3.Angle(rayDirection, transform.forward)-rayAngle) < fieldOfViewRange)
	 { // Detect if player is within the field of view
         if (Physics.Raycast (transform.position, rayDirection, hit))
          {
 
             if (hit.transform.tag == "Player") 
             {
                 Debug.Log(this.name + ": Can see player");
                 return true;
             }
             else
             {
                 //Debug.Log(this.name + ": Can not see player");
                 return false;
             }
         }
     }
 }

 function Update(){
 	CanSeePlayer();
 }


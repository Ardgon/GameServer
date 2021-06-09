using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI
{
    public class LaneMinion : Minion, ILaneMinion
    {
        /// <summary>
        /// Const waypoints that define the minion's route
        /// </summary>
        protected List<Vector2> _mainWaypoints;
        protected int _curMainWaypoint;
        public string BarracksName { get; } // barracks name of the position
        public MinionSpawnType MinionSpawnType { get; }

        public LaneMinion(
            Game game,
            MinionSpawnType spawnType,
            string barracksName,
            List<Vector2> mainWaypoints,
            string model,
            uint netId = 0,
            TeamId team = TeamId.TEAM_BLUE
        ) : base(game, null, new Vector2(), model, model, netId, team)
        {
            IsLaneMinion = true;
            MinionSpawnType = spawnType;
            BarracksName = barracksName;
            _mainWaypoints = mainWaypoints;
            _curMainWaypoint = 0;
            _aiPaused = false;

            var spawnSpecifics = _game.Map.MapProperties.GetMinionSpawnPosition(BarracksName);
            SetPosition(spawnSpecifics.Item2.X, spawnSpecifics.Item2.Y);

            _game.Map.MapProperties.SetMinionStats(this); // Let the map decide how strong this minion has to be.

            StopMovement();

            MoveOrder = OrderType.Hold;
            Replication = new ReplicationLaneMinion(this);
        }

        public LaneMinion(
            Game game,
            MinionSpawnType spawnType,
            string position,
            string model,
            uint netId = 0,
            TeamId team = TeamId.TEAM_BLUE
        ) : this(game, spawnType, position, new List<Vector2>(), model, netId, team)
        {
        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        public override void Update(float diff)
        {
            _actionDelay -= diff;
            base.Update(diff);
            _timeTargetting += diff / 1000.0f;
        }

        float _timeTargetting = 0f;
        float _actionDelay = 0.25f * 1000f;


        // TODO: Temporarily stop considering tarrget if failed to attack it for 4 seconds,
        // process calls for help without action delay, stop chasing if minions walks too far from lane
        public override bool AIMove()
        {

            // TODO: Process calls for help here, outside of action delay.

            // Minions take action on set timers to avoid overwhelming servers
            if (_actionDelay > 0)
            { 
                return false;  // TODO: verify what true/false means for AIMove
            }
            
            _actionDelay = 0.25f * 1000f;


            // TODO: Use unique LaneMinion AI instead of normal Minion AI and add here for return values.
            if (CanAttack() && ScanForTargets()) // returns true if we have a target
            {
                if (!RecalculateAttackPosition())
                {
                        KeepFocusingTarget(); // attack/follow target
                } 
                return false;
            }
            //else
            //{
                //SetTargetUnit(null, true);
                //CancelAutoAttack(false);
            //}

            // If we have lane path instructions from the map
            if (_mainWaypoints.Count > 0 && (TargetUnit == null || TargetUnit.IsDead) && CanMove())
            {
                SetTargetUnit(null, true);
                CancelAutoAttack(true);
                WalkToDestination();
            }
            return true;

        }

        public override void OnCollision(IGameObject collider, bool isTerrain = false)
        {
            base.OnCollision(collider, isTerrain);
        }


        override public bool ScanForTargets()
        {

            //if (TargetUnit != null && !TargetUnit.IsDead)
            //{
            //    return true;
            //}
            IAttackableUnit nextTarget = null;
            var nextTargetPriority = 14;
            var nearestObjects = _game.Map.CollisionHandler.QuadDynamic.GetObjects();
            //Find target closest to max attack range.

            var priority = 99;
            foreach (var it in nearestObjects.OrderBy(x => Vector2.DistanceSquared(Position, x.Position) - (DETECT_RANGE * DETECT_RANGE)))
            {
                if (!(it is IAttackableUnit u) ||
                    u.IsDead ||
                    u.Team == Team ||
                    //Vector2.DistanceSquared(Position, u.Position) > DETECT_RANGE * DETECT_RANGE ||
                    !_game.ObjectManager.TeamHasVisionOn(Team, u))
                {
                    continue;
                }

                if (Vector2.DistanceSquared(Position, u.Position) > DETECT_RANGE * DETECT_RANGE)
                {
                    break;
                }

                priority = (int)ClassifyTarget(u);  // get the priority.

                if (priority < nextTargetPriority) // if the priority is lower than the target we checked previously
                {
                    nextTarget = u;                // make it a potential target.
                    nextTargetPriority = priority;
                }

                if (priority == nextTargetPriority)
                {
                    //TODO: pick based on cheapest path
                }
            }

            if (TargetUnit != null && !TargetUnit.IsDead)
            {
                // Call for help priority
                if (priority < 6)
                {
                    //CancelAutoAttack(false);
                    SetTargetUnit(nextTarget, true);
                    return true;
                }                
            }


            if (nextTarget != null) // If we have a new target
            {
                // Set the new target and refresh waypoints
                if (TargetUnit != null && nextTarget.NetId != TargetUnit.NetId)
                {
                    //CancelAutoAttack(false);
                    SetTargetUnit(nextTarget, true);
                } else
                {
                    SetTargetUnit(nextTarget, true);
                }
                return true;
            } 

            return false;
        }

        public void WalkToDestination()
        {
            // TODO: Use the methods used in this function for any other minion pathfinding (attacking, targeting, etc).

            var mainWaypointCell = _game.Map.NavigationGrid.GetCellIndex(_mainWaypoints[_curMainWaypoint].X, _mainWaypoints[_curMainWaypoint].Y);
            var lastWaypointCell = 0;

            if (Waypoints.Count > 0)
            {
                lastWaypointCell = _game.Map.NavigationGrid.GetCellIndex(Waypoints[Waypoints.Count - 1].X, Waypoints[Waypoints.Count - 1].Y);
            }

            // First, we make sure we are pathfinding to our current main waypoint.
            if (lastWaypointCell != mainWaypointCell)
            {
                // Pathfind to the next waypoint.
                var path = new List<Vector2>() { Position, _mainWaypoints[_curMainWaypoint] };
                var tempPath = _game.Map.NavigationGrid.GetPath(Position, _mainWaypoints[_curMainWaypoint]);
                if (tempPath != null)
                {
                    path = tempPath;
                }
   
                SetWaypoints(path);
                UpdateMoveOrder(OrderType.MoveTo);

                //TODO: Here we need a certain way to tell if the Minion is in the path/lane, else use pathfinding to return to the lane.
                //I think in league when minion start chasing they save Current Position and
                //when it stop chasing the minion return to the last saved position, and then continue main waypoints from there.

                /*var path = _game.Map.NavGrid.GetPath(GetPosition(), _mainWaypoints[_curMainWaypoint + 1]);
                if(path.Count > 1)
                {
                    SetWaypoints(path);
                }*/
            }

            var waypointSuccessRange = CollisionRadius;
            var nearestObjects = _game.Map.CollisionHandler.QuadDynamic.GetNearestObjects(this);

            // This is equivalent to making any colliding minions equal to a single minion to save on pathfinding resources.
            foreach (IGameObject obj in nearestObjects)
            {
                if (obj is ILaneMinion minion)
                {
                    // If the closest minion is in collision range, add its collision radius to the waypoint success range.
                    if (GameServerCore.Extensions.IsVectorWithinRange(minion.Position, Position, waypointSuccessRange))
                    {
                        waypointSuccessRange += minion.CollisionRadius;
                    }
                    // If the closest minion (above) is not in collision range, then we stop the loop.
                    else
                    {
                        continue;
                    }
                }
            }

            // Since we are pathfinding to our current main waypoint, we check if 
            if (GameServerCore.Extensions.IsVectorWithinRange(Position, _mainWaypoints[_curMainWaypoint], waypointSuccessRange) && _mainWaypoints.Count > _curMainWaypoint + 1)
            {
                ++_curMainWaypoint;

                // Pathfind to the next waypoint.
                var path = new List<Vector2>() { Position, _mainWaypoints[_curMainWaypoint] };
                var tempPath = _game.Map.NavigationGrid.GetPath(Position, _mainWaypoints[_curMainWaypoint]);
                if (tempPath != null)
                {
                    path = tempPath;
                }

                SetWaypoints(path);
                UpdateMoveOrder(OrderType.AttackMove);
            }
        }

        // TODO: Override KeepFocusingTarget and use unique LaneMinion AI
    }
}

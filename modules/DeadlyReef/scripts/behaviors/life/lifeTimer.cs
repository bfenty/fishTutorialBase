if (!isObject(LifeTimerBehavior))
{
	%template = new BehaviorTemplate(LifeTimerBehavior);
	
	%template.friendlyName = "Lifetime with Burn";
	%template.behaviorType = "Life";
	%template.description = "Slowly lose hit points. Allow replenishing of hit points.";
	
	%template.addBehaviorField(burnRate, "milliseconds between timer-based point loss", int, 1000);
	%template.addBehaviorField(burnPoints, "points of life lost per period", float, 5.0);
}

function LifeTimerBehavior::onBehaviorAdd(%this)
{
	//Store our size on start-up as our "normal" size.
	//We will shrink or grow relative to this.
	
	%this.normalHeight = %this.owner.size.height;
	%this.normalWidth = %this.owner.size.width;
	
	//HP of life left
	%this.life = 100;
	
	%this.schedule(%this.burnRate, "lowerLife");
}

function LifeTimerBehavior::modifyLife(%this, %dmg)
{
	%this.life += %dmg;
	
	if(%this.life > 100)
	{
		%this.life = 100;
	}
	
	if(%this.life <=30)
	{
		%this.die();
	}
	else
	{
		%this.updateLifeSize();
	}
}

function LifeTimerBehavior::updateLifeSize(%this)
{
	%sizeRatio = %this.life / 100;
	
	%newWidth = %this.normalWidth * %sizeRatio;
	%newHeight = %this.normalHeight * %sizeRatio;
	
	%this.owner.setSize(%newWidth, %newHeight);
}

function LifeTimerBehavior::die(%this)
{
	%this.owner.setFlipY(true);
	%this.owner.setLinearVelocityX(0);
	%this.owner.setLinearVelocityY(10);
	%this.dead = true;
	%this.owner.blockControls();
}

function LifeTimerBehavior::lowerLife(%this)
{
	%this.modifyLife(-%this.burnPoints);
	
	if(! %this.dead)
	{
		%this.schedule(%this.burnRate, "lowerLife");
	}
}

function LifeTimerBehavior::onCollision(%this, %object, %collisionDetails)
{
	if (%object.class $= "FishFoodClass") {
		%this.modifyLife(%object.nutrition);
		%object.recycle();
	}
}

using System;
using System.Linq;
using Aertssen.Framework.Core.Builders;
using MyTemplate.Core.CQRS.MyEntity.Get;
using MyTemplate.Core.CQRS.MyEntity.List;
using MyTemplate.Core.CQRS.MyEntity.Store;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core.T4
{
    public partial class GetMyEntityQueryBuilder : ObjectBuilder<MyTemplate.Core.CQRS.MyEntity.Get.GetMyEntityQuery>
    {
        protected System.Int64 Id { get; set; }		
        public GetMyEntityQueryBuilder WithId(System.Int64 @id)
        {
            this.Id = @id;
            return this;
        }	

       
        public override GetMyEntityQuery Build()
        {
            var item = (GetMyEntityQuery)Activator.CreateInstance(typeof(GetMyEntityQuery));
    	
		
            item.Id = Id;
	    
            return item;
        }
      
    }

    public partial class GetMyEntityViewModelBuilder : ObjectBuilder<MyTemplate.Core.CQRS.MyEntity.Get.GetMyEntityViewModel>
    {
        protected MyTemplate.Dto.MyEntity.MyEntityDataDto MyEntity { get; set; }		
        public GetMyEntityViewModelBuilder WithMyEntity(MyTemplate.Dto.MyEntity.MyEntityDataDto @myentity)
        {
            this.MyEntity = @myentity;
            return this;
        }	

        public GetMyEntityViewModelBuilder WithMyEntity(Action<MyEntityDataDtoBuilder> myentityBuilder)
        {
            var b = new MyEntityDataDtoBuilder();
            myentityBuilder.Invoke(b);
            this.MyEntity = b.Build();
            return this;
        }



       
        public override GetMyEntityViewModel Build()
        {
            var item = (GetMyEntityViewModel)Activator.CreateInstance(typeof(GetMyEntityViewModel));
    	
		
            item.MyEntity = MyEntity;
	    
            return item;
        }
      
    }

    public partial class ListMyEntitiesQueryBuilder : ObjectBuilder<MyTemplate.Core.CQRS.MyEntity.List.ListMyEntitiesQuery>
    {
       
       
        public override ListMyEntitiesQuery Build()
        {
            var item = (ListMyEntitiesQuery)Activator.CreateInstance(typeof(ListMyEntitiesQuery));
    
            return item;
        }
      
    }

    public partial class StoreMyEntityCommandBuilder : ObjectBuilder<MyTemplate.Core.CQRS.MyEntity.Store.StoreMyEntityCommand>
    {
        protected MyTemplate.Dto.MyEntity.IStoreMyEntityDto MyEntity { get; set; }		
        public StoreMyEntityCommandBuilder WithMyEntity(MyTemplate.Dto.MyEntity.IStoreMyEntityDto @myentity)
        {
            this.MyEntity = @myentity;
            return this;
        }	

       
        public override StoreMyEntityCommand Build()
        {
            var item = (StoreMyEntityCommand)Activator.CreateInstance(typeof(StoreMyEntityCommand));
    	
		
            item.MyEntity = MyEntity;
	    
            return item;
        }
      
    }

    public partial class MyEntityDataDtoBuilder : ObjectBuilder<MyTemplate.Dto.MyEntity.MyEntityDataDto>
    {
        protected System.Int64 Id { get; set; }		
        public MyEntityDataDtoBuilder WithId(System.Int64 @id)
        {
            this.Id = @id;
            return this;
        }	
        protected System.String Name { get; set; }		
        public MyEntityDataDtoBuilder WithName(System.String @name)
        {
            this.Name = @name;
            return this;
        }	

       
        public override MyEntityDataDto Build()
        {
            var item = (MyEntityDataDto)Activator.CreateInstance(typeof(MyEntityDataDto));
    	
		
            item.Id = Id;
	    	
		
            item.Name = Name;
	    
            return item;
        }
      
    }

    public partial class StoreMyEntityDtoBuilder : ObjectBuilder<MyTemplate.Dto.MyEntity.StoreMyEntityDto>
    {
        protected System.String Name { get; set; }		
        public StoreMyEntityDtoBuilder WithName(System.String @name)
        {
            this.Name = @name;
            return this;
        }	

       
        public override StoreMyEntityDto Build()
        {
            var item = (StoreMyEntityDto)Activator.CreateInstance(typeof(StoreMyEntityDto));
    	
		
            item.Name = Name;
	    
            return item;
        }
      
    }
}
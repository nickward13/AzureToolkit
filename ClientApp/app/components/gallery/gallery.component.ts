import { Component, OnInit } from '@angular/core';
import { UserService } from '../../common/services/user.service';
import { User } from '../../common/models/user';
import { AzureToolkitService } from '../../common/services/azureToolkit.service';
import { SavedImage } from '../../common/models/savedImage';

@Component({
    selector: 'gallery',
    templateUrl: './gallery.component.html',
    styleUrls: ['./gallery.component.css']
})
export class GalleryComponent implements OnInit {
    user: User | null = null;
    savedImages: SavedImage[] | null = null;

    constructor(private userService: UserService, private azureToolkitService: AzureToolkitService) { }

    ngOnInit(): void {
        this.userService.getUser().subscribe(user => this.user = user);
        console.log(this.user);
        if(this.user != null){
            this.azureToolkitService.getImages(this.user.userId).subscribe(images => this.savedImages = images);
        }
    }
}
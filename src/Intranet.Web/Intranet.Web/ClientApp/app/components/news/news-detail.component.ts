﻿import { Component, OnInit } from '@angular/core'
import { RouterModule, Router, ActivatedRoute } from '@angular/router'
import { Location } from '@angular/common'
import { DataService } from '../../shared/data_services/data.service'

import NewsItem from '../../models/newsItem.model'

@Component({
    selector: 'news-detail',
    templateUrl: 'news-detail.component.html',
    styleUrls: ['./news.component.css']
})

export class NewsDetailComponent implements OnInit {
    id: number
    title: string
    date: Date
    text: string = ''
    author: string

    newsItem: NewsItem
    newsItems: NewsItem[]
    selectedNewsItem: NewsItem
    info: string = ''
    newsItemDeleted: boolean = false

    constructor(
        private dataService: DataService,
        private route: ActivatedRoute,
        private location: Location,
    ) {
        this.newsItem = new NewsItem()
    }

    goBack(): void {
        this.location.back()
    }

    ngOnInit() {
        this.id = +this.route.snapshot.params['id']

        this.dataService.getNewsItem(this.id).subscribe(
            (newsitem: NewsItem) => {
                this.title = newsitem.title
                this.date = newsitem.date
                this.text = newsitem.text
                this.author = newsitem.author
                this.newsItem = newsitem
            },
            error => {
                console.log('Failed while trying to load specific newsitem of news' + error)
            }
        )
    }

    onSelect(newsitem: NewsItem): void {
        this.selectedNewsItem = newsitem
    }

    removeNewsItem(newsitem: NewsItem) {
        this.selectedNewsItem = newsitem
        this.dataService.deleteNewsItem(this.selectedNewsItem.id)
            .subscribe(() => {
                this.newsItemDeleted = true
                this.info = this.newsItem.title + ' was deleted successfully!'
                console.log('News was deleted successfully!')
                this.dataService.getNewsItems().subscribe(
                        (newsitems: NewsItem[]) => {
                            this.newsItems = newsitems
                        },
                        error => {
                            console.log('Failed to load news ' + error)
                        }
                    )
            })
    }

    getUrlToImage(urls: string[]) {
        return urls.find(value => {
            return value.includes('600/300')
        })
    }
}